using IzTek.Carbon.Footprint.Domain.Events.User;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

/// <summary>
/// Kullanıcı ağaç bağışı yaptığında:
/// Kullanıcıya bağış teşekkür bildirimi gönderir
/// </summary>
public class TreesDonatedDomainEventHandler(IPlatformService platformService)
{
    public async Task Handle(TreesDonatedDomainEvent @event, CancellationToken ct)
    {
        await platformService.SendPushToUserAsync(
            userId: @event.UserId.ToString(),
            title: "Bağışınız İçin Teşekkürler! 🌳",
            body: $"{@event.TreeCount} ağaç bağışladınız. Doğaya katkınız için teşekkürler!",
            data: new { treeCount = @event.TreeCount, pointsSpent = @event.PointsSpent });
    }
}