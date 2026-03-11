using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

public static class UserScoreUpdatedDomainEventHandler
{
    public static async Task Handle(
        UserScoreUpdatedDomainEvent @event,
        IApplicationDbContext context,
        ILogger<UserScoreUpdatedDomainEvent> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Updating score for user {UserId}. Increment: {Score}",
            @event.UserId, @event.Score);

        var userGuid = Guid.Parse(@event.UserId);

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userGuid, ct);

        if (user is null)
        {
            logger.LogWarning("User {UserId} not found for score update.", @event.UserId);
            return;
        }

        // ✅ TotalCarbonPoint → TotalPoints (User entity ile uyumlu)
        user.UpdateMonthlyCarbonResult(@event.Score);

        await context.SaveChangesAsync(ct);

        logger.LogInformation("Score updated for user {UserId}. New Total: {Total}",
            @event.UserId, @event.Score);
    }
}