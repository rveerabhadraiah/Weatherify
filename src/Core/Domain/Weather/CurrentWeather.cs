namespace Domain.Weather;

public class CurrentWeather: Entity
{
  public Coordinates Coordinates { get; set; } = new Coordinates(); 
  public List<WeatherMain> WeatherMain { get; set; } = new();
  public Temperature Temperature { get; set; } = new Temperature();
  public string CityName { get; set; } = string.Empty;
}