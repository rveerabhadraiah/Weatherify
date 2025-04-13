using System.Text.Json;
using Domain.Auth;
using Infrastructure.ApiResponse.Music;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.MusicService.Auth;

public interface ITokenStore
{
  Task<TokenResult> GetTokenAsync(string userId);
  Task SaveTokenAsync(string userId, TokenResult token);
}

public class DistributedCacheTokenStore(IDistributedCache cache) : ITokenStore
{
  private readonly IDistributedCache _cache = cache;
  private const string TokenKeyPrefix = "spotify_token_";
  
  public async Task<TokenResult> GetTokenAsync(string userId)
  {
    var key = GetTokenKey(userId);
    var tokenJson = await _cache.GetStringAsync(key);

    if (string.IsNullOrWhiteSpace(tokenJson))
      return TokenResult.Failed;

    var tokenResult = JsonSerializer.Deserialize<TokenResult>(tokenJson);
    if (tokenResult == null)
      return TokenResult.Failed;
    
    return tokenResult;
  }

  public async Task SaveTokenAsync(string userId, TokenResult tokenResult)
  {
    var token = tokenResult.Token ?? Token.Empty;
    var key = GetTokenKey(userId);
    
    var tokenLifetime = token.ExpiresAt - DateTime.UtcNow;
    var options = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = tokenLifetime.Add(TimeSpan.FromHours(1))
    };
    
    var tokenResultJson = JsonSerializer.Serialize(tokenResult);
    await _cache.SetStringAsync(key, tokenResultJson, options);
  }

  private string GetTokenKey(string userId)
  {
    return $"{TokenKeyPrefix}{userId}";
  }
}