using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

public class UserDeletedDomainEventHandler(
    IApplicationDbContext context,
    ILogger<UserDeletedDomainEventHandler> logger)
{
    public async Task Handle(UserDeletedDomainEvent @event, CancellationToken ct)
    {
        logger.LogInformation("User deleted: {UserId} at {DeletedAt}", @event.UserId, @event.DeletedAt);

        // KVKK — UserPollResults'taki ad soyad snapshot'larını anonimleştir
        var pollResults = await context.UserPollResults
            .Where(x => x.UserId == @event.UserId)
            .ToListAsync(ct);

        foreach (var result in pollResults)
            result.Anonymize();

        await context.SaveChangesAsync(ct);
    }
}