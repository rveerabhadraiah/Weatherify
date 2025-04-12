using Infrastructure.Configs;
using Microsoft.Extensions.Options;

namespace Infrastructure.ApiHandlers;

public class WeatherApiHandler(IOptions<WeatherApiOptions> options) : DelegatingHandler
{
  private readonly string _apiKey = options.Value.ApiKey;

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    if (request.RequestUri == null) throw new ArgumentNullException(nameof(request.RequestUri));
    
    request.Headers.Add("x-api-key", _apiKey);
    return await base.SendAsync(request, cancellationToken);
  }
}