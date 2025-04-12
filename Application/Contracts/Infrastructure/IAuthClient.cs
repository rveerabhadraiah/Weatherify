namespace Application.Contracts.Infrastructure;

public interface IAuthClient
{
  Task<string> GetAuthorizationUrlAsync();
  Task HandleCallbackAsync(string code, string state);
  Task<bool> IsAuthorizedAsync();
}