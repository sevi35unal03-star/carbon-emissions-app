using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Domain.Events.Poll;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.EventHandlers;

/// <summary>
/// Kullanıcı anketi tamamladığında:
/// 1. Liderboard cache'ini invalidate eder
/// 2. O ayki hedefin tamamlanıp tamamlanmadığını kontrol eder
/// </summary>
public class PollAnsweredDomainEventHandler(
    ICacheService cacheService,
    IApplicationDbContext context)
{
    public async Task Handle(PollAnsweredDomainEvent @event, CancellationToken ct)
    {
        // 1. Liderboard cache invalidation
        await cacheService.RemoveAsync(
            $"monthly-leaderboard:{@event.Month}:{@event.Year}", ct);

        // 2. O aya ait hedefi getir
        var goal = await context.Goals
            .FirstOrDefaultAsync(g => g.Month == @event.Month &&
                                      g.Year == @event.Year, ct);

        if (goal is null || goal.IsCompleted)
            return;

        // 3. O ay toplam bağışlanan ağaç sayısını hesapla
        var totalTrees = await context.UserPollResults
            .Where(r => r.Month == @event.Month && r.Year == @event.Year)
            .SumAsync(r => r.TreeCount, ct);

        // 4. Hedefe ulaşıldıysa tamamla
        if (totalTrees >= goal.TargetTreeCount)
        {
            goal.Complete();
            await context.SaveChangesAsync(ct);
        }
    }
}