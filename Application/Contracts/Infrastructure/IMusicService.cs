namespace Application.Contracts.Infrastructure;

public interface IMusicService
{
  Task GetUserPlayListAsync(string userId);
}