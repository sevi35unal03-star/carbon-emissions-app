using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

/// <summary>
/// Kullanıcı silindiğinde (soft delete):
/// 1. Kullanıcıya ait tüm cache'leri temizler
/// 2. Silme işlemini loglar
/// </summary>
public class UserDeletedDomainEventHandler(
    ICacheService cacheService,
    ILogger<UserDeletedDomainEventHandler> logger)
{
    public async Task Handle(UserDeletedDomainEvent @event, CancellationToken ct)
    {
        logger.LogInformation("User deleted: {UserId} at {DeletedAt}", @event.UserId, @event.DeletedAt);

        // Kullanıcıya ait tüm cache'leri temizle
        await cacheService.RemoveAsync($"user-profile:{@event.UserId}", ct);
        await cacheService.RemoveByPrefixAsync($"pending-questions:{@event.UserId}", ct);
    }
}