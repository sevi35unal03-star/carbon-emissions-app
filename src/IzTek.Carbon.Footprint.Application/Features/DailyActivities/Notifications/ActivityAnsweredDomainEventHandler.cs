using IzTek.Carbon.Footprint.Domain.Events.Activity;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Notifications;

/// <summary>
/// Kullanıcı günlük aktivite sorusunu cevapladığında:
/// 1. O kullanıcının takvim cache'ini invalidate eder
/// 2. Pending soru cache'ini invalidate eder
/// </summary>
public class ActivityAnsweredDomainEventHandler(ICacheService cacheService)
{
    public async Task Handle(ActivityAnsweredDomainEvent @event, CancellationToken ct)
    {
        var month = @event.AnsweredAt.Month;
        var year = @event.AnsweredAt.Year;

        // Period 1 ve Period 2 cache'lerini invalidate et
        await cacheService.RemoveAsync(
            $"activity-calendar:{year}:{month}", ct);

        // Yıllık cache invalidation
        await cacheService.RemoveAsync(
            $"activity-calendar:{year}", ct);

        // Pending soru cache invalidation
        await cacheService.RemoveAsync(
            $"pending-questions:{@event.UserId}", ct);
    }
}