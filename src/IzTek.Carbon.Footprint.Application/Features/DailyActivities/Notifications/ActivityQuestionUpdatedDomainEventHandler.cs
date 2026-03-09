using System.Globalization;
using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SendPush;
using IzTek.Carbon.Footprint.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.EventHandlers;

public class ActivityQuestionUpdatedDomainEventHandler(
    ILogger<ActivityQuestionUpdatedDomainEventHandler> logger,
    IApplicationDbContext context,
    ICacheService cacheService,
    INotificationService notificationService,
    IMessageContext bus, 
    IStringLocalizer<Resources> localizer)
{
    public async Task Handle(ActivityQuestionUpdatedDomainEvent @event, CancellationToken ct)
    {
        logger.LogInformation("Processing Question Update Event: {Id}", @event.Id);

        // 1. Özellik: Cache Temizleme
        await cacheService.RemoveAsync("daily_questions_active");

        // --- 2. ÖZELLİK: WOLVERINE ZAMANLAMA (Benim eklediğim kısım) ---
        // Soru güncellendiğinde, belirlenen saat için yeni bir bildirim planlıyoruz.
        var now = DateTime.Now.TimeOfDay;
        var delay = @event.ScheduledTime - now;

        if (delay.TotalSeconds <= 0)
            delay = delay.Add(TimeSpan.FromDays(1));

        // Bu komut, belirlenen 'delay' süresi sonunda SendQuestionPushNotificationHandler'ı tetikler.
        await bus.ScheduleAsync(new SendQuestionPushNotificationCommand(@event.Id, @event.Text), delay);

        logger.LogInformation("Future notification scheduled for {Time} (In {Delay} minutes)",
            @event.ScheduledTime, Math.Round(delay.TotalMinutes, 2));
        // -----------------------------------------------------------

        // 3. Özellik: Anlık Bilgilendirme (Senin mevcut kodun)
        // Eğer kullanıcıya "Bir soru güncellendi" diye ANLIK bildirim gitmesini istiyorsan burası kalmalı.
        var languageGroups = await context.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new { u.PollQuestionId, u.LanguageCode })
            .GroupBy(u => u.LanguageCode)
            .ToListAsync(ct);

        foreach (var group in languageGroups)
        {
            var languageCode = group.Key ?? "tr-TR";
            var userIds = group.Select(x => x.Id.ToString()).ToList();

            using (new CultureScope(languageCode))
            {
                string title = localizer["DailyQuestionUpdatedTitle"];
                // @event.Text veya @event içindeki diğer property'leri kullanabilirsin
                string message = localizer["DailyQuestionUpdatedMsg", @event.Text];

                await notificationService.SendToUsersAsync(
                    userIds: userIds,
                    title: title,
                    message: message,
                    data: new { QuestionId = @event.Id }
                );
            }
        }

        logger.LogInformation("Localized notifications dispatched for {Count} language groups.", languageGroups.Count);
    }
}