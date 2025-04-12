namespace Infrastructure.ApiResponse.Music;

public class TokenResult
{
  public bool Success { get; init; }
  public string? AccessToken { get; init; }
  public string? RefreshToken { get; init; }
  public DateTime ExpiresAt { get; init; }
  public string? Error { get; init; }
}