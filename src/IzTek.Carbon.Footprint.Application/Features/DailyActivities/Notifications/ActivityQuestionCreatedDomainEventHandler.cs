using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SendPush;
using IzTek.Carbon.Footprint.Domain.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Notifications;

public static class ActivityQuestionCreatedDomainEventHandler
{
    public static async Task HandleAsync(
        ActivityQuestionCreatedDomainEvent @event,
        IMessageContext bus,
        ILogger<ActivityQuestionCreatedDomainEvent> logger,
        CancellationToken cancellationToken)
    {
        // 1. Hesaplama: Şu an ile planlanan saat arasındaki farkı bul
        var now = DateTime.Now.TimeOfDay;
        var delay = @event.ScheduledTime - now;

        // Eğer planlanan saat bugün geçtiyse, yarın için planla (Opsiyonel mantık)
        if (delay.TotalSeconds <= 0)
        {
            delay = delay.Add(TimeSpan.FromDays(1));
        }

        try
        {
            // 2. Wolverine Scheduled Messaging
            // Bu satır, SendQuestionPushNotificationCommand'ı 'delay' süresi kadar bekletip sonra kuyruğa atar.
            await bus.ScheduleAsync(
                new SendQuestionPushNotificationCommand(@event.Id, @event.Text),
                delay);

            logger.LogInformation("Push notification for Question {Id} scheduled in {Delay} minutes.",
                @event.Id, delay.TotalMinutes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to schedule push notification.");
        }
    }
}