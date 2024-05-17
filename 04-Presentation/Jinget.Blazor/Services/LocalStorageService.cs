namespace Jinget.Blazor.Services;

public class LocalStorageService(IJSRuntime js) : ILocalStorageService
{
    public async Task<string> GetItemAsync(string key) => await js.InvokeAsync<string>("localStorage.getItem", key).ConfigureAwait(false);

    public async Task SetItemAsync(string key, string value) => await js.InvokeVoidAsync("localStorage.setItem", key, value).ConfigureAwait(false);

    public async Task RemoveItemAsync(string key) => await js.InvokeVoidAsync("localStorage.removeItem", key).ConfigureAwait(false);
}