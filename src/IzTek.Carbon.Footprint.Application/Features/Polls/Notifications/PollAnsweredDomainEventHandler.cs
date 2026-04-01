using Microsoft.Extensions.Caching.Memory;
using IzTek.Carbon.Footprint.Domain.Events.Poll;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.EventHandlers;

/// <summary>
/// Kullanıcı anketi tamamladığında:
/// 1. Liderboard cache'ini invalidate eder
/// 2. O ayki hedefin tamamlanıp tamamlanmadığını kontrol eder
/// </summary>
public class PollAnsweredDomainEventHandler(
    IMemoryCache cache,
    IApplicationDbContext context)
{
    public async Task Handle(PollAnsweredDomainEvent @event, CancellationToken ct)
    {
        // 1. Liderboard cache invalidation
        cache.Remove($"monthly-leaderboard:{@event.Month}:{@event.Year}");

        // 2. O aya ait hedefi getir
        var goal = await context.Goals
            .FirstOrDefaultAsync(g => g.Month == @event.Month &&
                                      g.Year == @event.Year, ct);

        if (goal is null || goal.IsCompleted)
            return;

        // 3. O ay toplam bağışlanan ağaç sayısını hesapla
        var totalTrees = await context.TreeDonations
            .Where(r => r.DonationDate.Month == @event.Month
                     && r.DonationDate.Year == @event.Year)
            .SumAsync(r => r.TreeCount, ct);

        // 4. Hedefe ulaşıldıysa tamamla
        if (totalTrees >= goal.TargetTreeCount)
        {
            goal.Complete();
            await context.SaveChangesAsync(ct);
        }
    }
}