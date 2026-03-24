namespace IzTek.Carbon.Footprint.Application.Common.Extensions;

public static class CacheInvalidationExtensions
{
    public static async Task InvalidateAsync(
        this ICacheService cache,
        ICacheInvalidator command,
        CancellationToken ct = default)
    {
        foreach (var key in command.CacheKeys)
        {
            // Prefix ile bitiyor mu? (örn: "activity-calendar:userId:")
            if (key.EndsWith(':'))
                await cache.RemoveByPrefixAsync(key, ct);
            else
                await cache.RemoveAsync(key, ct);
        }
    }
}