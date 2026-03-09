namespace IzTek.Carbon.Footprint.Application.Common.Behaviors;

public class CachingBehavior(ICacheService cache)
{
    public async Task<T?> HandleAsync<T>(
        ICacheableQuery query,
        Func<Task<T?>> next,
        CancellationToken ct)
    {
        // 1. Cache'de var mı?
        var cached = await cache.GetAsync<T>(query.CacheKey, ct);
        if (cached is not null)
            return cached;

        // 2. Yoksa handler'ı çalıştır
        var result = await next();

        // 3. Sonucu cache'e yaz
        if (result is not null)
            await cache.SetAsync(query.CacheKey, result, query.Expiry, ct);

        return result;
    }
}

