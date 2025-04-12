namespace Infrastructure.MusicService.Auth;

public interface ITokenStore
{
  Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt)> GetTokenAsync();
  Task SaveTokenAsync(string accessToken, string refreshToken, DateTime expiresAt);
}