using Newtonsoft.Json;

namespace Jinget.Blazor.Services;

public abstract class BrowserStorageService(IJSRuntime jsRuntime, string storage) : IBrowserStorageService
{
    protected IJSRuntime Js { get; } = jsRuntime;
    protected string Storage { get; set; } = storage;

    public async Task<string> GetItemAsync(string key) => await Js.InvokeAsync<string>($"{Storage}.getItem", key).ConfigureAwait(false);

    public async Task SetItemAsync(string key, string value) => await Js.InvokeVoidAsync($"{Storage}.setItem", key, value).ConfigureAwait(false);

    public async Task RemoveItemAsync(string key) => await Js.InvokeVoidAsync($"{Storage}.removeItem", key).ConfigureAwait(false);

    public async Task UpsertItemAsync(string key, string value)
    {
        await RemoveItemAsync(key).ConfigureAwait(false);
        await SetItemAsync(key, value).ConfigureAwait(false);
    }

    public async Task<Dictionary<string, string>?> GetAllAsync()
    {
        var stringfiedItems = await Js.InvokeAsync<string>($"getAll_{Storage}Keys").ConfigureAwait(false);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(stringfiedItems);
    }

    public async Task RemoveAllAsync() => await Js.InvokeVoidAsync($"removeAll_{Storage}Keys").ConfigureAwait(false);
}