using IzTek.Carbon.Footprint.Domain.Events.User;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

/// <summary>
/// Kullanıcı ağaç bağışı yaptığında:
/// 1. Kullanıcı profil cache'ini invalidate eder
/// 2. Liderboard cache'ini invalidate eder
/// 3. Kullanıcıya bağış teşekkür bildirimi gönderir
/// </summary>
public class TreesDonatedDomainEventHandler(
    ICacheService cacheService,
    IPlatformService platformService)
{
    public async Task Handle(TreesDonatedDomainEvent @event, CancellationToken ct)
    {
        var now = @event.DonationDate;

        // 1. Kullanıcı profil cache invalidation
        await cacheService.RemoveAsync($"user-profile:{@event.UserId}", ct);

        // 2. Liderboard cache invalidation
        await cacheService.RemoveAsync($"monthly-leaderboard:{now.Month}:{now.Year}", ct);

        // 3. Kullanıcıya teşekkür bildirimi
        await platformService.SendPushToUserAsync(
            userId: @event.UserId.ToString(),
            title: "Bağışınız İçin Teşekkürler! 🌳",
            body: $"{@event.TreeCount} ağaç bağışladınız. Doğaya katkınız için teşekkürler!",
            data: new { treeCount = @event.TreeCount, pointsSpent = @event.PointsSpent });
    }
}