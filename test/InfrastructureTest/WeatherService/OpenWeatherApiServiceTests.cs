using Application.Contracts.Infrastructure;
using Infrastructure;
using Infrastructure.Configs;
using Infrastructure.WeatherService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace InfrastructureTest.WeatherService;

public class OpenWeatherApiServiceTests
{
  private readonly IWeatherService _weatherService;

  public OpenWeatherApiServiceTests()
  {
    var configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json")
      .AddUserSecrets<OpenWeatherApiServiceTests>()
      .AddEnvironmentVariables()
      .Build();
    
    var apiKey = configuration["WeatherApi:ApiKey"];
    var options = Options.Create(new WeatherApiOptions
    {
      BaseUrl = configuration["WeatherApi:BaseUrl"],
      ApiKey = apiKey
    });
    
    var httpClient = new HttpClient
    {
      BaseAddress = new Uri(options.Value.BaseUrl ?? "https://api.openweathermap.org/data/2.5/"),
      DefaultRequestHeaders = { {"x-api-key", options.Value.ApiKey } }
    };
    
    _weatherService = new OpenWeatherApiService(httpClient, options);
  }
  
  [Test]
  public async Task GetCurrentWeatherAsync_ShouldReturnWeatherData()
  {
    // Arrange
    const double latitude = 13.0006; // Example latitude
    const double longitude = 77.5334; // Example longitude

    // Act
    var result = await _weatherService.GetCurrentWeatherAsync(latitude, longitude);

    // Assert
    Assert.NotNull(result);
  }
  
}