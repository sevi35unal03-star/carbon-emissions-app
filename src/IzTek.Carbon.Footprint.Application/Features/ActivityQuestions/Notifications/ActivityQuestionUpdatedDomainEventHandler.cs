using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Domain.Events.Activity;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.EventHandlers;

/// <summary>
/// Aktivite sorusu güncellendiğinde:
/// 1. Soru listesi cache'ini invalidate eder
/// 2. Tüm kullanıcılara güncel bildirim gönderir
/// </summary>
public class ActivityQuestionUpdatedDomainEventHandler(
    ICacheService cacheService,
    IPlatformService platformService)
{
    public async Task Handle(ActivityQuestionUpdatedDomainEvent @event, CancellationToken ct)
    {
        // 1. Soru listesi cache invalidation
        await cacheService.RemoveByPrefixAsync("activity-questions", ct);

        // 2. Tüm kullanıcılara güncel bildirim gönder
        await platformService.SendPushToAllUsersAsync(
            title: "Aktivite Sorusu Güncellendi",
            body: @event.Text,
            data: new { questionId = @event.Id, scheduledTime = @event.ScheduledTime });
    }
}