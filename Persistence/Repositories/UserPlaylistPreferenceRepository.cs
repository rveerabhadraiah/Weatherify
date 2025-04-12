using Application.Contracts.Persistence;

namespace Persistence.Repositories;

public class UserPlaylistPreferenceRepository: IUserPlaylistPreference
{
  public void AddPlaylistPreference(string userId, string weatherCondition, string playlistId)
  {
    throw new NotImplementedException();
  }

  public Task RemovePlaylistPreferenceAsync(string userId, string weatherCondition, string playlistId)
  {
    throw new NotImplementedException();
  }

  public Task<List<string>> GetPlaylistPreferencesAsync(string userId, string weatherCondition)
  {
    throw new NotImplementedException();
  }
  
}