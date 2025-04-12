using Infrastructure.Configs;
using Infrastructure.MusicService;
using Infrastructure.MusicService.Auth;
using Microsoft.Extensions.Configuration;

namespace InfrastructureTest.MusicService;



public class SpotifyAuthServiceTest
{
  // private readonly SpotifyAuthService _spotifyAuthService;
  //
  // public SpotifyAuthServiceTest()
  // {
  //   var configuration = new ConfigurationBuilder()
  //     .AddJsonFile("appsettings.json")
  //     .Build();
  //   var spotifyOptions = configuration.GetSection("SpotifyApi").Get<SpotifyApiOptions>();
  //   var httpClient = new HttpClient();
  //   _spotifyAuthService = new SpotifyAuthService(httpClient, spotifyOptions);
  // }
  //
  // [Test]
  // public async Task Test()
  // {
  //   var codeVerifier = _spotifyAuthService.GenerateCodeVerifier();
  //   var codeChallenge = _spotifyAuthService.GenerateCodeChallenge(codeVerifier);
  //   var authorizationUrl = _spotifyAuthService.GenerateAuthorizationUrl(codeChallenge);
  //   var code = "";
  //   var exchangeCodeForTokensAsync =  await _spotifyAuthService.ExchangeCodeForTokenAsync(code, codeVerifier);
  //   var refreshTokenAsync = _spotifyAuthService.RefreshAccessTokenAsync(exchangeCodeForTokensAsync.RefreshToken);
  // }
}