namespace Jinget.Blazor.Services;

public class TokenStorageService(ILocalStorageService localStorage, string key) : ITokenStorageService
{
    public async Task SetTokenAsync(string token) => await localStorage.SetItemAsync(key, token).ConfigureAwait(false);

    public async Task<string> GetTokenAsync()
    {
        try
        {
            return await localStorage.GetItemAsync(key).ConfigureAwait(false);
        }
        catch
        {
            return await Task.FromResult("").ConfigureAwait(false);
        }
    }

    public async Task RemoveTokenAsync() => await localStorage.RemoveItemAsync(key);
}