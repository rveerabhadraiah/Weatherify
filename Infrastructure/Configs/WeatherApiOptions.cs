namespace Infrastructure.Configs;

public class WeatherApiOptions
{
  public required string? ApiKey { get; init; }
  public required string? BaseUrl { get; init; }
}