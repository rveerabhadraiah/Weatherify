namespace Domain;

public record UserWeatherPlaylistPreference (
  string WeatherCondition,
  string UserId,
  string UserEmail,
  string PlayListUrl);
