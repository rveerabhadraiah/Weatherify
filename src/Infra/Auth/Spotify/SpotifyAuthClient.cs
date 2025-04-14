using System.Net.Http.Json;
using Application.Contracts.Infrastructure;
using Domain.Auth;

namespace Auth.Spotify;

public class SpotifyAuthClient(HttpClient httpClient) : IAuthClient
{
  public async Task<string> GetAuthorizationUrlAsync()
  {
    var response = await httpClient.GetStringAsync("api/SpotifyAuth/authz-url");
    return response;
  }

  public async Task HandleCallbackAsync(string code, string state)
  {
    var response = await httpClient.GetAsync($"api/SpotifyAuth/callback?code={Uri.EscapeDataString(code)}&state={Uri.EscapeDataString(state)}");
  }

  public async Task<bool> IsAuthorizedAsync()
  {
    try
    {
      var response = await httpClient.GetAsync("api/SpotifyAuth/is-authorized");
      response.EnsureSuccessStatusCode();

      var authStatus = await response.Content.ReadFromJsonAsync<AuthorizationStatus>();
      return authStatus?.IsAuthorized ?? false;
    }
    catch
    {
      return false; 
    }
  }
}