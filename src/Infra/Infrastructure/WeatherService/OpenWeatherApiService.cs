using Application.Contracts.Infrastructure;
using Application.Exceptions.Weather;
using Domain.Weather;
using Infrastructure.ApiResponse.Weather;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Infrastructure.Configs;

namespace Infrastructure.WeatherService;

public class OpenWeatherApiService : IWeatherService
{
  private readonly HttpClient _httpClient;

  public OpenWeatherApiService(HttpClient httpClient, IOptions<WeatherApiOptions> options)
  {
    var options1 = options?.Value ?? throw new ArgumentNullException(nameof(options));
    var baseUrl = options1.BaseUrl ?? throw new ArgumentNullException(nameof(options1.BaseUrl));
    
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _httpClient.BaseAddress = new Uri(baseUrl);
  }

  public async Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude)
  {
    try
    {
      var requestUri = $"weather?lat={latitude}&lon={longitude}&units=metric";
      var response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();

      var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
      var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

      if (weatherData == null)
        throw new InvalidOperationException("Failed to deserialize weather data");

      return MapToDomainEntity(weatherData);
    }
    catch (HttpRequestException e)
    {
      throw new WeatherServiceException("Error fetching weather data", e);
    }
    catch (JsonException e)
    {
      throw new WeatherServiceException("Error deserializing weather data", e);
    }
  }

  private CurrentWeather MapToDomainEntity(WeatherApiResponse apiResponse)
  {
    if (apiResponse.Coord == null || apiResponse.Weather == null || apiResponse.Main == null || apiResponse.Name == null)
      throw new InvalidOperationException("Invalid weather data received");

    var weather = new CurrentWeather
    {
      Coordinates = new Coordinates
      {
        Latitude = apiResponse.Coord.Lat,
        Longitude = apiResponse.Coord.Lon
      },
      WeatherMain = apiResponse.Weather.Select(w => new WeatherMain
      {
        Id = w.Id,
        Main = w.Main,
        Description = w.Description,
        Icon = w.Icon
      }).ToList(),
      Temperature = new Temperature
      {
        CurrentTemp = apiResponse.Main.Temp,
        FeelsLike = apiResponse.Main.FeelsLike
      },
      CityName = apiResponse.Name
    };
    return weather;
  }
}