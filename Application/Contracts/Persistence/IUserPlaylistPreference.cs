namespace Application.Contracts.Persistence;

public interface IUserPlaylistPreference
{
  void AddPlaylistPreference(string userId, string weatherCondition, string playlistId);
  Task RemovePlaylistPreferenceAsync(string userId, string weatherCondition, string playlistId);
  Task<List<string>> GetPlaylistPreferencesAsync(string userId, string weatherCondition);
}