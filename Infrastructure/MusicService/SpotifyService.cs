using Application.Contracts.Infrastructure;
using Infrastructure.MusicService.Auth;

namespace Infrastructure.MusicService;

public class SpotifyService(HttpClient httpClient, ISpotifyAuthService authService, ITokenStore tokenStore)
  : IMusicService
{
  private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
  private readonly ISpotifyAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));
  private readonly ITokenStore _tokenStore =  tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
  
  public async Task GetUserPlayListAsync(string userId)
  {
    throw new NotImplementedException();
  }
}