namespace Jinget.Core.Utilities;

/// <summary>
/// Provides utility methods for generating cache keys.
/// </summary>
public static class CachingUtility
{
    /// <summary>
    /// Generates a cache key based on the model type, cache entry type, and optional ID.
    /// </summary>
    /// <param name="modelType">The type of the model for which the cache key is generated.</param>
    /// <param name="type">The type of cache entry.</param>
    /// <param name="id">Optional ID for specific item caching.</param>
    /// <returns>The generated cache key.</returns>
    public static string GetKeyName(Type modelType, CacheEntryType type, object? id = null)
    {
        // Attempts to retrieve the cache type identifier from the model's attribute.
        var cacheTypeName = modelType.GetCustomAttribute<CacheTypeIdentifier>()?.Identifier.Replace("ViewModel", "").Replace("Model", "") ?? string.Empty;

        // If the identifier is not found in the attribute, uses the model's name.
        if (string.IsNullOrWhiteSpace(cacheTypeName))
        {
            cacheTypeName = $"{modelType.Name.Replace("ViewModel", "").Replace("Model", "")}";
        }

        // Constructs the cache key based on the cache entry type.
        return type == CacheEntryType.SpecificItemWithId
            ? $"{cacheTypeName}_{CacheEntryType.SpecificItemWithId.GetDescription()}_{id}".ToLower()
            : $"{cacheTypeName}_{type.GetDescription()}".ToLower();
    }
}