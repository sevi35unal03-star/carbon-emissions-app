using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Domain.Events;
using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

public static class UserScoreUpdatedDomainEventHandler
{
    public static async Task HandleAsync(
        UserScoreUpdatedDomainEvent @event,
        IApplicationDbContext context,
        ILogger<UserScoreUpdatedDomainEvent> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating score for user {UserId}. Increment: {Score}", @event.UserId, @event.Score);

        try
        {
            // 1. Kullanıcıyı bul (Guid dönüşümü gerekebilir)
            var userGuid = Guid.Parse(@event.UserId);
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.PollQuestionId == userGuid, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} not found for score update.", @event.UserId);
                return;
            }

            // 2. Puanı güncelle
            user.TotalCarbonPoint += @event.Score;

            // 3. Opsiyonel: Seviye atlama (Level Up) mantığı buraya gelebilir
            // CheckLevelUp(user);

            // 4. Değişikliği kaydet
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully updated score for user {UserId}. New Total: {Total}",
                @event.UserId, user.TotalCarbonPoint);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating score for user {UserId}", @event.UserId);
        }
    }
}