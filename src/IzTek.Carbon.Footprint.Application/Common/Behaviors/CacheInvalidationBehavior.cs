namespace IzTek.Carbon.Footprint.Application.Common.Behaviors;


public class CacheInvalidationBehavior(ICacheService cache)
{
    public async Task HandleAsync(
        ICacheInvalidator command,
        Func<Task> next,
        CancellationToken ct)
    {
        // 1. Önce command'ı çalıştır
        await next();

        // 2. Sonra cache'i temizle
        foreach (var key in command.CacheKeys)
            await cache.RemoveAsync(key, ct);
    }
}