namespace IzTek.Carbon.Footprint.Application.Common.Behaviors;

public class CacheInvalidationBehavior(ICacheService cache)
{
    public async Task Finally(
        ICacheInvalidator command,
        CancellationToken ct)
    {
        // Handler başarıyla çalıştıktan sonra cache temizle
        foreach (var key in command.CacheKeys)
            await cache.RemoveAsync(key, ct);
    }
}