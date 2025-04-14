namespace Application.Exceptions.Weather;

public class WeatherServiceException : Exception
{
  public WeatherServiceException(string message): base(message)
  {
  }
  public WeatherServiceException(string message, Exception ex) : base(message, ex)
  {
  }
}