using Auth.Spotify;
using Domain.Auth;

namespace Auth.Spotify;

public class SpotifyApiHandler(ITokenStore tokenStore, ISpotifyAuthService spotifyAuthService) : DelegatingHandler
{
  private readonly ITokenStore _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
  private readonly ISpotifyAuthService _spotifyAuthService = spotifyAuthService ?? throw new ArgumentNullException(nameof(spotifyAuthService));

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    if (request.RequestUri == null) throw new ArgumentNullException(nameof(request.RequestUri));
    
    await EnsureAccessTokenAsync();
    
    var tokenResult = await _tokenStore.GetTokenAsync("roshan");
    var token = tokenResult.Token ?? Token.Empty;


    if(string.IsNullOrWhiteSpace(token.AccessToken) || DateTime.UtcNow >= token.ExpiresAt)
      throw new InvalidOperationException($"No access token available. User must authenticate");
    
    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
    return await base.SendAsync(request, cancellationToken);
  }
  
  private async Task EnsureAccessTokenAsync()
  {
    var tokenResult = await _tokenStore.GetTokenAsync("roshan");
    var token = tokenResult.Token ?? Token.Empty;


    if (string.IsNullOrWhiteSpace(token.AccessToken) || DateTime.UtcNow >= token.ExpiresAt)
    {
      if (string.IsNullOrEmpty(token.RefreshToken))
        throw new InvalidOperationException("No refresh token available. User must authenticate");
            
      var refreshTokenResult = await _spotifyAuthService.RefreshAccessTokenAsync(token.RefreshToken);
      if (!refreshTokenResult.Success)
        throw new InvalidOperationException($"Failed to refresh access token: {refreshTokenResult.Error}");
      
      await _tokenStore.SaveTokenAsync("roshan", refreshTokenResult);
    }
  }
}