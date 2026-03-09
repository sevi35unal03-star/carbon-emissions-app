using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var data = await cache.GetStringAsync(key, ct);
        return data is null
            ? default
            : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
        };
        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value),
            options,
            ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await cache.RemoveAsync(key, ct);

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        // Redis'te prefix ile silmek için IConnectionMultiplexer kullan
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{prefix}*");
        foreach (var key in keys)
            await cache.RemoveAsync(key!, ct);
    }
}