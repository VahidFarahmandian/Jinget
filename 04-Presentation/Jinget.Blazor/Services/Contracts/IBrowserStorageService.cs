namespace Jinget.Blazor.Services.Contracts;

public interface IBrowserStorageService
{
    /// <summary>
    /// get item with specific key from storage
    /// </summary>
    Task<string> GetItemAsync(string key);

    /// <summary>
    /// get all items from storage
    /// </summary>
    Task<Dictionary<string, string>?> GetAllAsync();

    /// <summary>
    /// set item to storage
    /// </summary>
    Task SetItemAsync(string key, string value);

    /// <summary>
    /// add or update item to storage
    /// </summary>
    Task UpsertItemAsync(string key, string value);

    /// <summary>
    /// remove item with specific key from storage
    /// </summary>
    Task RemoveItemAsync(string key);

    /// <summary>
    /// remove all items from storage
    /// </summary>
    Task RemoveAllAsync();
}