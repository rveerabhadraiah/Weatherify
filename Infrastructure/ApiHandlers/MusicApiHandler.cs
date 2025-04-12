using Infrastructure.MusicService.Auth;

namespace Infrastructure.ApiHandlers;

public class MusicApiHandler(ITokenStore tokenStore, ISpotifyAuthService spotifyAuthService) : DelegatingHandler
{
  private readonly ITokenStore _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
  private readonly ISpotifyAuthService _spotifyAuthService = spotifyAuthService ?? throw new ArgumentNullException(nameof(spotifyAuthService));

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    if (request.RequestUri == null) throw new ArgumentNullException(nameof(request.RequestUri));
    
    await EnsureAccessTokenAsync();
    
    var tokens = await _tokenStore.GetTokenAsync();
    if(string.IsNullOrWhiteSpace(tokens.AccessToken) || DateTime.UtcNow >= tokens.ExpiresAt)
      throw new InvalidOperationException($"No access token available. User must authenticate");
    
    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);
    return await base.SendAsync(request, cancellationToken);
  }
  
  private async Task EnsureAccessTokenAsync()
  {
    var tokens = await _tokenStore.GetTokenAsync();

    if (string.IsNullOrWhiteSpace(tokens.AccessToken) || DateTime.UtcNow >= tokens.ExpiresAt)
    {
      if (string.IsNullOrEmpty(tokens.RefreshToken))
        throw new InvalidOperationException("No refresh token available. User must authenticate");
            
      var result = await _spotifyAuthService.RefreshAccessTokenAsync(tokens.RefreshToken);
      if (!result.Success)
        throw new InvalidOperationException($"Failed to refresh access token: {result.Error}");
            
      await _tokenStore.SaveTokenAsync(result.AccessToken, result.RefreshToken, result.ExpiresAt);
    }
  }
}