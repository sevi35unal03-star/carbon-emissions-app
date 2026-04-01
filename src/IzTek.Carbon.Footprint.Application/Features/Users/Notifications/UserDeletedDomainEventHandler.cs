using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

/// <summary>
/// Kullanıcı silindiğinde (soft delete) silme işlemini loglar
/// </summary>
public class UserDeletedDomainEventHandler(ILogger<UserDeletedDomainEventHandler> logger)
{
    public async Task Handle(UserDeletedDomainEvent @event, CancellationToken ct)
    {
        logger.LogInformation("User deleted: {UserId} at {DeletedAt}", @event.UserId, @event.DeletedAt);
    }
}