using Microsoft.Extensions.Caching.Distributed;

namespace Auth.Spotify;

public interface IOAuthStateStore
{
  Task SaveStateAsync(string state, string codeVerifier);
  Task<string?> GetCodeVerifierAsync(string state);
  Task RemoveStateAsync(string state);
}

public class DistributedCacheOAuthStateStore(IDistributedCache cache) : IOAuthStateStore
{
  private readonly IDistributedCache _cache = cache;
  private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

  public async Task SaveStateAsync(string state, string codeVerifier)
  {
    await _cache.SetStringAsync(
      $"spotify_state:{state}",
      codeVerifier,
      new DistributedCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = _cacheDuration
      });
  }

  public Task<string?> GetCodeVerifierAsync(string state)
  {
    return _cache.GetStringAsync($"spotify_state:{state}");
  }

  public async Task RemoveStateAsync(string state)
  {
    await _cache.RemoveAsync($"spotify_state:{state}");
  }
}