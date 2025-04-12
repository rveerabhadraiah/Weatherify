using Infrastructure.MusicService.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpotifyAuthController(ITokenStore tokenStore, ISpotifyAuthService spotifyAuthService)
  : ControllerBase
{
  [HttpGet("authz-url")]
  public ActionResult<string> GetAuthorizationUrl()
  {
    var codeVerifier = spotifyAuthService.GenerateCodeVerifier();
    var codeChallenge = spotifyAuthService.GenerateCodeChallenge(codeVerifier);
    var state = Guid.NewGuid().ToString();

    HttpContext.Session.SetString("spotify_code_verifier", codeVerifier);
    HttpContext.Session.SetString("spotify_state", state);

    var authUrl = spotifyAuthService.GenerateAuthorizationUrl(codeChallenge, state);
    return Ok(authUrl);
  }

  [HttpGet("callback")]
  public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state, [FromQuery] string error)
  {
    if (!string.IsNullOrWhiteSpace(error))
    {
      return BadRequest($"Authorization failed: {error}");
    }

    var originalState = HttpContext.Session.GetString("spotify_state");
    if (originalState != state)
      return BadRequest($"State mismatch. Possible CSRF attack");

    var codeVerifier = HttpContext.Session.GetString("spotify_code_verifier");
    if (string.IsNullOrWhiteSpace(codeVerifier))
      return BadRequest($"code verifier not found");

    var tokenResult = await spotifyAuthService.ExchangeCodeForTokenAsync(code, codeVerifier);

    if (!tokenResult.Success)
      return BadRequest($"Failed to exchange code : {tokenResult.Error}");


    await tokenStore.SaveTokenAsync(tokenResult.AccessToken, tokenResult.RefreshToken, tokenResult.ExpiresAt);

    HttpContext.Session.Remove("spotify_code_verifier");
    HttpContext.Session.Remove("spotify_state");

    return Ok();
  }

  [HttpGet("is-authorized")]
  public async Task<ActionResult<bool>> IsAuthorized()
  {
    var(accessToken, _, expiresAt) =  await tokenStore.GetTokenAsync();
    return Ok(!string.IsNullOrWhiteSpace(accessToken) && expiresAt > DateTime.UtcNow);
  }
  
}