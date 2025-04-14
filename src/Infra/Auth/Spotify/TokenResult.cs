using Domain.Auth;

namespace Auth.Spotify;

public class TokenResult
{
  public bool Success { get; init; }
  public Token? Token { get; init; }
  public string? Error { get; init; }

  public static TokenResult Failed => new()
  {
    Success = false,
    Token = Token.Empty,
    Error = "Failed to retrieve token"
  };
  
  public static TokenResult SuccessFull(Token token) => new()
  {
    Success = true,
    Token = token,
    Error = string.Empty,
  };
}