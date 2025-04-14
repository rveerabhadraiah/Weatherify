using Domain.Weather;

namespace Application.Contracts.Infrastructure;

public interface IWeatherService
{
  Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude);
}