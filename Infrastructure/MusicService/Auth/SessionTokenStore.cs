using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.MusicService.Auth;

public class SessionTokenStore(IHttpContextAccessor httpContextAccessor) : ITokenStore
{
  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
  private const string SessionTokenKey = "SpotifySessionToken";


  public Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt)> GetTokenAsync()
  {
    var session = _httpContextAccessor.HttpContext?.Session;
    if (session == null)
      return Task.FromResult((string.Empty, string.Empty, DateTime.MinValue));

    var tokensJson = string.Empty;
    if (session.TryGetValue(SessionTokenKey, out var sessionToken))
      tokensJson = System.Text.Encoding.UTF8.GetString(sessionToken);

    if (string.IsNullOrEmpty(tokensJson))
      return Task.FromResult((string.Empty, string.Empty, DateTime.MinValue));

    var tokens = JsonSerializer.Deserialize<SpotifyTokens>(tokensJson);
    if (tokens == null)
      throw new NullReferenceException("tokens is null");

    return Task.FromResult((tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresAt));
  }

  public Task SaveTokenAsync(string accessToken, string refreshToken, DateTime expiresAt)
  {
    var session = _httpContextAccessor.HttpContext?.Session;
    if (session == null)
      throw new InvalidOperationException("HTTP session is not available");

    var tokens = new SpotifyTokens
    {
      AccessToken = accessToken,
      RefreshToken = refreshToken,
      ExpiresAt = expiresAt
    };

    var tokensJson = JsonSerializer.Serialize(tokens);
    session.Set(SessionTokenKey, System.Text.Encoding.UTF8.GetBytes(tokensJson));

    return Task.CompletedTask;
  }

  private class SpotifyTokens
  {
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTime ExpiresAt { get; init; }
  }
}