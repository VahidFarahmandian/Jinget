namespace Jinget.Blazor.Services.Contracts;

public interface ILocalStorageService
{
    Task<string> GetItemAsync(string key);
    Task SetItemAsync(string key, string value);
    Task RemoveItemAsync(string key);
}