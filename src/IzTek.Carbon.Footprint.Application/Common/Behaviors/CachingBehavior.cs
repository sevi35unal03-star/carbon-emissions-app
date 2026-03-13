using Wolverine.Runtime.Handlers;

namespace IzTek.Carbon.Footprint.Application.Common.Behaviors;

public class CachingBehavior(ICacheService cache)
{
    private object? _cachedResult;

    public async Task<HandlerContinuation> Before<T>(
        ICacheableQuery query,
        IMessageContext context,
        CancellationToken ct)
    {
        var cached = await cache.GetAsync<T>(query.CacheKey, ct);
        if (cached is not null)
        {
            _cachedResult = cached;
            return HandlerContinuation.Stop; // Cache hit — handler'ı atla
        }
        return HandlerContinuation.Continue; // Cache miss — devam et
    }

    public async Task Finally<T>(
        ICacheableQuery query,
        T? result,
        CancellationToken ct)
    {
        if (result is not null && _cachedResult is null)
            await cache.SetAsync(query.CacheKey, result, query.Expiry, ct);
    }
}