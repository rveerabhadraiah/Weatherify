namespace Domain.Weather
{
  public class WeatherMain
  {
    
    public int Id { get; set; }
    // weather condition
    public string? Main { get; set; }

    // weather condition description
    public string? Description { get; set; }
    
    public string? Icon { get; set; }
  }
}
