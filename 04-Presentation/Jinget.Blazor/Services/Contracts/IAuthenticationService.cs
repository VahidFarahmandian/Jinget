namespace Jinget.Blazor.Services.Contracts;

public interface IAuthenticationService
{
    Task<string> LoginAsync(string username, string password);
    Task<string> GenerateTokenAsync(IEnumerable<Claim> claims, string authenticationType);
}