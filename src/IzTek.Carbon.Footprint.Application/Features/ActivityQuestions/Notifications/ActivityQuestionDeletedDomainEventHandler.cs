using IzTek.Carbon.Footprint.Domain.Events.Activity;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Notifications;

public class ActivityQuestionDeletedDomainEventHandler(
    ILogger<ActivityQuestionDeletedDomainEventHandler> logger,
    ICacheService cacheService)
{
    public async Task Handle(ActivityQuestionDeletedDomainEvent @event, CancellationToken ct)
    {
        logger.LogInformation("Processing Question Deletion Event for ID: {Id}", @event.Id);

        await cacheService.RemoveAsync("daily_questions_active");

        // 2. Wolverine Bilgilendirmesi: 
        // Not: Wolverine'de schedule edilen mesajlar genellikle ID ile iptal edilebilir 
        // veya SendPushNotificationHandler çalışırken sorunun veritabanında olup olmadığını kontrol eder.

        logger.LogInformation("Cache invalidated for deleted question {Id}.", @event.Id);
    }
}