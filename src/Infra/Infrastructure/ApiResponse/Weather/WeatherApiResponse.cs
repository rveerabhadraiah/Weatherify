using System.Text.Json.Serialization;

namespace Infrastructure.ApiResponse.Weather;

public class WeatherApiResponse
{
  public Coord? Coord { get; init; }
  public List<Weather>? Weather { get; init; }
  public Main? Main { get; init; }
  public string? Name { get; init; }
}

public class Coord
{
  public double Lon { get; set; }
  public double Lat { get; set; }
}

public class Weather
{
  public int Id { get; set; }
  public string? Main { get; set; }
  public string? Description { get; set; }
  public string? Icon { get; set; }
}

public class Main
{
  public double Temp { get; set; }
  
  [JsonPropertyName(("feels_like"))]
  public double FeelsLike { get; set; }
}

