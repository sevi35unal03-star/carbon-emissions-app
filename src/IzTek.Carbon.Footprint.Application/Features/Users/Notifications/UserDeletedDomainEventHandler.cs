using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

public class UserDeletedDomainEventHandler
{
    public void Handle(UserDeletedDomainEvent @event, ILogger<UserDeletedDomainEventHandler> logger)
    {
        logger.LogWarning("User {UserId} has self-deleted their account at {Time}",
            @event.UserId, @event.DeletedAt);
    }
}