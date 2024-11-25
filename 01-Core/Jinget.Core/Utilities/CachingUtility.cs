namespace Jinget.Core.Utilities;

public static class CachingUtility
{
    public static string GetKeyName(Type modelType, CacheEntryType type, object? id = null)
    {
        var cacheTypeName = modelType.GetCustomAttribute<CacheTypeIdentifier>()?.Identifier.Replace("ViewModel", "")
            .Replace("Model", "") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(cacheTypeName))
        {
            cacheTypeName = $"{modelType.Name.Replace("ViewModel", "").Replace("Model", "")}";
        }

        return type == CacheEntryType.SpecificItemWithId
            ? $"{cacheTypeName}_{CacheEntryType.SpecificItemWithId.GetDescription()}_{id}".ToLower()
            : $"{cacheTypeName}_{type.GetDescription()}".ToLower();
    }
}