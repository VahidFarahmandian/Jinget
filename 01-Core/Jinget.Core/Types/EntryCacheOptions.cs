using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jinget.Core.Types;

public class EntryCacheOptions
{
    public bool Enabled { get; set; } = true;
    public string? Key { get; set; }
    public CacheEntryType Type { get; set; }

    public DistributedCacheEntryOptions EntryOptions { get; set; } = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        SlidingExpiration = TimeSpan.FromMinutes(5),
        AbsoluteExpiration = DateTime.UtcNow.AddMinutes(10),
    };

    public static readonly JsonSerializerOptions DefaultDeserializationOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public static readonly JsonSerializerOptions DefaultSerializationOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}