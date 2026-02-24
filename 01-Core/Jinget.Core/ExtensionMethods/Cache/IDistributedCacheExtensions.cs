using Microsoft.Extensions.Caching.Distributed;

using System.Threading;

namespace Jinget.Core.ExtensionMethods.Cache
{
    public static class IDistributedCacheExtensions
    {
        /// <summary>
        /// store keys in a key store
        /// </summary>
        public static async Task SaveKeySetAsync(
            this IDistributedCache cache, string keySetName, string keyName, int expiration = 60, string keySeparator = ",", CancellationToken cancellationToken = default)
        {
            var stringfiedKeys = await cache.GetStringAsync(keySetName, cancellationToken) ?? "";
            var keys = stringfiedKeys.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!keys.Contains(keyName))
            {
                keys.Add(keyName);
                await cache.SetStringAsync(keySetName, string.Join(keySeparator, keys), new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(expiration)
                }, token: cancellationToken);
            }
        }

        public static async Task RemoveAllAsync(this IDistributedCache cache, string keySetName, string keySeparator = ",", CancellationToken cancellationToken = default)
        {
            var inventorytypeKeys = await cache.GetStringAsync(keySetName, cancellationToken) ?? "";
            var keys = inventorytypeKeys.Split(keySeparator, StringSplitOptions.RemoveEmptyEntries).ToList();

            await cache.RemoveRangeAsync(keys, cancellationToken);
        }

        public static async Task RemoveRangeAsync(this IDistributedCache cache, List<string> keys, CancellationToken cancellationToken = default)
        {
            foreach (var key in keys)
            {
                await cache.RemoveAsync(key, cancellationToken);
            }
        }
    }
}
