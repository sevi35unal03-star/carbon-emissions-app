using IzTek.Carbon.Footprint.Domain.Events.Activity;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Notifications;

/// <summary>
/// Kullanıcı günlük aktivite sorusunu cevapladığında:
/// 1. O kullanıcının takvim cache'ini invalidate eder
/// 2. Pending soru cache'ini invalidate eder
/// </summary>
public class ActivityAnsweredDomainEventHandler
{
    public Task Handle(ActivityAnsweredDomainEvent @event, CancellationToken ct)
    {
        // Cache kaldırıldı — invalidation artık burada yapılmıyor.
        // Takvim ve pending soru verileri her zaman DB'den taze çekilecek.
        return Task.CompletedTask;
    }
}