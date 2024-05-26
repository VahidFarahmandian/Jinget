namespace Jinget.Blazor.Services.Contracts;

public interface IAuthenticationService
{
    Task<string> LoginAsync(string username, string password);
}