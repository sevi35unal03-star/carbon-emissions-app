namespace IzTek.Carbon.Footprint.Application.Common.Extensions;

public static class CacheExtensions
{
    /// <summary>
    /// Cache'de varsa Result olarak döner, yoksa null döner.
    /// Kullanım: if (await cache.GetCachedResultAsync<T>(key, ct) is { } hit) return hit;
    /// </summary>
    public static async Task<Result<T>?> GetCachedResultAsync<T>(
        this ICacheService cache,
        string key,
        CancellationToken ct = default)
    {
        var cached = await cache.GetAsync<T>(key, ct);
        return cached is not null ? Result<T>.Success(cached) : null;
    }

    /// <summary>
    /// Sonuç başarılıysa cache'e yazar.
    /// </summary>
    public static async Task SetCachedResultAsync<T>(
        this ICacheService cache,
        string key,
        Result<T> result,
        TimeSpan? expiry = null,
        CancellationToken ct = default)
    {
        if (result.IsSuccessful && result.Data is not null)
            await cache.SetAsync(key, result.Data, expiry, ct);
    }
}