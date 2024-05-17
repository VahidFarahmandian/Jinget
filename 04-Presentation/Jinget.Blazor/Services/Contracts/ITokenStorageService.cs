namespace Jinget.Blazor.Services.Contracts;

public interface ITokenStorageService
{
    Task SetTokenAsync(string token);
    Task<string> GetTokenAsync();
    Task RemoveTokenAsync();
}