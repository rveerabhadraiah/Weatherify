namespace Domain.Auth;

public class Token
{
  public string? AccessToken { get; set; }
  public string? RefreshToken { get; set; }
  public DateTime ExpiresAt { get; set; }
  
  public bool IsValid => !string.IsNullOrWhiteSpace(AccessToken) && ExpiresAt > DateTime.UtcNow;
  
  public static Token Empty => new()
  {
    AccessToken = string.Empty,
    RefreshToken = string.Empty,
    ExpiresAt = DateTime.MinValue
  };
}