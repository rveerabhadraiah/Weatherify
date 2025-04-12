using System.Net.Http.Json;
using Application.Contracts.Infrastructure;

namespace Infrastructure.MusicService.Auth;

public class SpotifyAuthClient(HttpClient httpClient) : IAuthClient
{
  public async Task<string> GetAuthorizationUrlAsync()
  {
    var response = await httpClient.GetStringAsync("api/SpotifyAuth/authz-url");
    return response;
  }

  public async Task HandleCallbackAsync(string code, string state)
  {
    await httpClient.GetAsync($"api/SpotifyAuth/callback?code={Uri.EscapeDataString(code)}&state={Uri.EscapeDataString(state)}");
  }

  public async Task<bool> IsAuthorizedAsync()
  {
    var response = await httpClient.GetAsync($"api/SpotifyAuth/is-authorized");
    return await response.Content.ReadFromJsonAsync<bool>();
  }
}