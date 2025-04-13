using Domain.Auth;
using Infrastructure.ApiResponse.Music;
using Infrastructure.MusicService.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpotifyAuthController(
  ITokenStore tokenStore, 
  ISpotifyAuthService spotifyAuthService,
  IOAuthStateStore stateStore,
  ILogger<SpotifyAuthController> logger) : ControllerBase
{
  [HttpGet("authz-url")]
  public async Task<ActionResult<string>> GetAuthorizationUrl()
  {
    try
    {

      var codeVerifier = spotifyAuthService.GenerateCodeVerifier();
      var codeChallenge = spotifyAuthService.GenerateCodeChallenge(codeVerifier);
      var state = Guid.NewGuid().ToString();  
      await stateStore.SaveStateAsync(state, codeVerifier);
      
      var authUrl = spotifyAuthService.GenerateAuthorizationUrl(codeChallenge, state);
      return Ok(authUrl);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error generating authorization URL");
      return BadRequest($"Error : {ex.Message}");
    }
  }

  [HttpGet("callback")]
  public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state, [FromQuery] string? error = null)
  {
    logger.LogInformation($"Callback received with code: {code}, state: {state}, error: {error}");
    
    if (!string.IsNullOrWhiteSpace(error))
      return BadRequest($"Authorization failed: {error}");
    
    var codeVerifier = await stateStore.GetCodeVerifierAsync(state);
    if (string.IsNullOrWhiteSpace(codeVerifier))
      return BadRequest($"Invalid state parameter. Possible CSRF attack or state expired");

    try
    {
      var tokenResult = await spotifyAuthService.ExchangeCodeForTokenAsync(code, codeVerifier);

      if (!tokenResult.Success)
      {
        logger.LogWarning($"Token exchange failed: {tokenResult.Error}");
        return BadRequest($"Failed to exchange code: {tokenResult.Error}");
      }
      
      await tokenStore.SaveTokenAsync("roshan", tokenResult);

      // Clean up session

      await stateStore.RemoveStateAsync(state);

      return Ok(new { Success = true });
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error processing callback");
      return BadRequest($"Error: {ex.Message}");
    }
  }

  [HttpGet("is-authorized")]
  public async Task<ActionResult<bool>> IsAuthorized()
  {
    try
    {
      TokenResult tokenResult = await tokenStore.GetTokenAsync("roshan");
      var authorizationStatus = new AuthorizationStatus{IsAuthorized = !string.IsNullOrEmpty(tokenResult.Token.AccessToken) };
      return Ok(authorizationStatus);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error checking auth status");
      return BadRequest($"Error: {ex.Message}");
    }
  }
  
}