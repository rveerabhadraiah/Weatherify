using Application.Contracts.Infrastructure;
using Infrastructure.ApiHandlers;
using Infrastructure.Configs;
using Infrastructure.MusicService.Auth;
using Infrastructure.WeatherService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<WeatherApiOptions>(builder.Configuration.GetSection("WeatherApi"));
builder.Services.Configure<SpotifyApiOptions>(builder.Configuration.GetSection("SpotifyApi"));

// Spotify Services
builder.Services.AddTransient<MusicApiHandler>();
builder.Services.AddScoped<ISpotifyAuthService, SpotifyAuthService>();
builder.Services.AddScoped<ITokenStore, SessionTokenStore>();

// Weather Services
builder.Services.AddTransient<WeatherApiHandler>();
builder.Services.AddHttpClient<IWeatherService, OpenWeatherApiService>().AddHttpMessageHandler<WeatherApiHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseSession();
app.UseRouting();
app.MapControllers();
app.Run();
