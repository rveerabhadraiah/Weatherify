namespace Infrastructure.Configs;

public class SpotifyApiOptions
{
  public string? ClientId { get; init; }
  public string? BaseUrl { get; init; }
  public string? RedirectUri { get; init; }
  public string? Scope { get; init; }
}