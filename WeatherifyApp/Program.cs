using Application.Contracts.Infrastructure;
using Infrastructure.MusicService.Auth;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WeatherifyApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<IAuthClient, SpotifyAuthClient>(client => {
  client.BaseAddress = new Uri("http://localhost:5019/");
});

var app = builder.Build();


await builder.Build().RunAsync();