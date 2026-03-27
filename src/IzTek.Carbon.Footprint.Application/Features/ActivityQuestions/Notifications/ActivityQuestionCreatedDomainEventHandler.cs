using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Domain.Events.Activity;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.EventHandlers;

/// <summary>
/// Yeni aktivite sorusu oluşturulduğunda:
/// 1. Soru listesi cache'ini invalidate eder
/// 2. Tüm kullanıcılara push notification gönderir
/// </summary>
public class ActivityQuestionCreatedDomainEventHandler(
    ICacheService cacheService,
    IPlatformService platformService)
{
    public async Task Handle(ActivityQuestionCreatedDomainEvent @event, CancellationToken ct)
    {
        // 1. Soru listesi cache invalidation
        await cacheService.RemoveByPrefixAsync("activity-questions", ct);

        // 2. Tüm kullanıcılara bildirim gönder
        await platformService.SendPushToAllUsersAsync(
            title: "Yeni Aktivite Sorusu",
            body: @event.Text,
            data: new { questionId = @event.Id, scheduledTime = @event.ScheduledTime });
    }
 
}