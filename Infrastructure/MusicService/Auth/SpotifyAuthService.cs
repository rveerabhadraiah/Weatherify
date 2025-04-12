using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Infrastructure.ApiResponse.Music;
using Infrastructure.Configs;
using Microsoft.Extensions.Options;

namespace Infrastructure.MusicService.Auth;

// https://developer.spotify.com/documentation/web-api/tutorials/code-pkce-flow

public interface ISpotifyAuthService
{
  string GenerateCodeVerifier();
  string GenerateCodeChallenge(string codeVerifier);
  string GenerateAuthorizationUrl(string codeChallenge, string? state = null);
  Task<TokenResult> ExchangeCodeForTokenAsync(string code, string codeVerifier);
  Task<TokenResult> RefreshAccessTokenAsync(string refreshToken);
}


public class SpotifyAuthService(HttpClient httpClient, IOptions<SpotifyApiOptions> spotifyApiOptions) : ISpotifyAuthService
{
  private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
  private readonly SpotifyApiOptions _spotifyApiOptions = spotifyApiOptions.Value ?? throw new ArgumentNullException(nameof(spotifyApiOptions));

  public string GenerateCodeVerifier()
  {
    var randomBytes = new byte[64];
    using (var rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(randomBytes);
    }
            
    return Convert.ToBase64String(randomBytes)
      .TrimEnd('=')
      .Replace('+', '-')
      .Replace('/', '_');
  }

  public string GenerateCodeChallenge(string codeVerifier)
  {
    if (string.IsNullOrEmpty(codeVerifier))
      throw new ArgumentNullException(nameof(codeVerifier));

    using var sha256 = SHA256.Create();
    var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
    return Convert.ToBase64String(challengeBytes)
      .TrimEnd('=')
      .Replace('+', '-')
      .Replace('/', '_');
  }
  
  // Get the authorization URL for the user to login
  public string GenerateAuthorizationUrl(string codeChallenge, string? state = null)
  {
    if (string.IsNullOrEmpty(codeChallenge))
      throw new ArgumentNullException(nameof(codeChallenge));
    
    if (_spotifyApiOptions.ClientId == null || _spotifyApiOptions.Scope == null || _spotifyApiOptions.RedirectUri == null)
      throw new ArgumentException("invalid spotify config");
                
    var parameters = new Dictionary<string, string>
    {
      { "client_id", _spotifyApiOptions.ClientId },
      { "response_type", "code" },
      { "redirect_uri", _spotifyApiOptions.RedirectUri },
      { "code_challenge_method", "S256" },
      { "code_challenge", codeChallenge },
      { "scope", _spotifyApiOptions.Scope },
    };

    if (!string.IsNullOrEmpty(state))
    {
      parameters.Add("state", state);
    }

    var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    return $"https://accounts.spotify.com/authorize?{queryString}";
  }

  public async Task<TokenResult> ExchangeCodeForTokenAsync(string code, string codeVerifier)
  {
    if (string.IsNullOrEmpty(code))
      throw new ArgumentNullException(nameof(code));
                
    if (string.IsNullOrEmpty(codeVerifier))
      throw new ArgumentNullException(nameof(codeVerifier));
    
    if (_spotifyApiOptions.ClientId == null || _spotifyApiOptions.RedirectUri == null)
      throw new ArgumentException("invalid spotify config ");
                
    var tokenRequestParams = new Dictionary<string, string>
    {
      { "client_id", _spotifyApiOptions.ClientId },
      { "grant_type", "authorization_code" },
      { "code", code },
      { "redirect_uri", _spotifyApiOptions.RedirectUri },
      { "code_verifier", codeVerifier }
    };

    return await ExecuteTokenRequestAsync(tokenRequestParams);
  }

  // Refresh the access token using the refresh token
  public async Task<TokenResult> RefreshAccessTokenAsync(string refreshToken)
  {
    if (string.IsNullOrEmpty(refreshToken))
      throw new ArgumentNullException(nameof(refreshToken));
    
    if (_spotifyApiOptions.ClientId == null)
      throw new ArgumentException("invalid spotify config ");

                
    var refreshParams = new Dictionary<string, string>
    {
      { "client_id", _spotifyApiOptions.ClientId },
      { "grant_type", "refresh_token" },
      { "refresh_token", refreshToken }
    };

    return await ExecuteTokenRequestAsync(refreshParams);
  }
  
  // Helper method to execute token requests
  private async Task<TokenResult> ExecuteTokenRequestAsync(Dictionary<string, string> parameters)
  {
    try
    {
      var requestContent = new FormUrlEncodedContent(parameters);
      var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", requestContent);
                
      var responseContent = await response.Content.ReadAsStringAsync();
                
      if (!response.IsSuccessStatusCode)
      {
        return new TokenResult
        {
          Success = false,
          Error = $"Error: {response.StatusCode}, Details: {responseContent}"
        };
      }

      var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
      if (tokenResponse == null)
        throw new NullReferenceException("token response is null");
                
      return new TokenResult
      {
        Success = true,
        AccessToken = tokenResponse.AccessToken,
        RefreshToken = tokenResponse.RefreshToken ?? parameters.GetValueOrDefault("refresh_token"),
        ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
      };
    }
    catch (Exception ex)
    {
      return new TokenResult
      {
        Success = false,
        Error = $"Exception: {ex.Message}"
      };
    }
  }
  
  
}