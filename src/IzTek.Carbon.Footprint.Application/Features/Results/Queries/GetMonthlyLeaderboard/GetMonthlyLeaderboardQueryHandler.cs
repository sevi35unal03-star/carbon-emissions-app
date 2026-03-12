using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public class GetMonthlyLeaderboardQueryHandler
{
    public async Task<Result<GetMonthlyLeaderboardResponse>> Handle(
        GetMonthlyLeaderboardQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        // 1. Aylık ve yıllık hedef
        var monthlyGoal = await context.Goals
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Month == query.Month && x.Year == query.Year, ct);

        var yearlyTarget = await context.Goals
            .AsNoTracking()
            .Where(x => x.Year == query.Year)
            .SumAsync(x => x.TargetTreeCount, ct);

        var monthlyTarget = monthlyGoal?.TargetTreeCount ?? 0;

        // 2. O aya ait toplam bağış — "X ağaç kaldı" hesabı için
        var totalDonatedThisMonth = await context.TreeDonations
            .AsNoTracking()
            .Where(x => x.DonationDate.Month == query.Month
                     && x.DonationDate.Year == query.Year)
            .SumAsync(x => x.TreeCount, ct);

        // 3. O aya ait bağışlara göre kullanıcı bazlı sıralama
        var rankingsRaw = await (
            from d in context.TreeDonations.AsNoTracking()
            where d.DonationDate.Month == query.Month
               && d.DonationDate.Year == query.Year
            group d by d.UserId into g
            join u in context.Users on g.Key equals u.Id
            orderby g.Sum(x => x.TreeCount) descending
            select new
            {
                UserId = g.Key,
                TotalTrees = g.Sum(x => x.TreeCount),
                FullName = u.Name + " " + u.Surname
            })
            .ToListAsync(ct);

        var currentUserId = currentUser.UserId;

        // 4. Sıralı liderboard listesi
        var leaders = rankingsRaw
            .Select((x, i) => new LeaderboardItemDto(
                Rank: i + 1,
                FullName: x.FullName,
                TreeCount: x.TotalTrees,
                IsCurrentUser: x.UserId == currentUserId))
            .ToList();

        // 5. Giriş yapan kullanıcının sırası ve mesajı
        var userEntry = rankingsRaw
            .Select((x, i) => new { x.UserId, x.TotalTrees, Rank = i + 1 })
            .FirstOrDefault(x => x.UserId == currentUserId);

        var userRankDto = userEntry is not null
            ? new UserRankDto(
                Rank: userEntry.Rank,
                TreeCount: userEntry.TotalTrees,
                Message: $"{userEntry.TotalTrees} Ağaç ile {userEntry.Rank}. sıradasınız.")
            : null;

        return Result<GetMonthlyLeaderboardResponse>.Success(new GetMonthlyLeaderboardResponse(
            YearlyTargetTreeCount: yearlyTarget,
            MonthlyTargetTreeCount: monthlyTarget,
            RemainingTreeCount: Math.Max(0, monthlyTarget - totalDonatedThisMonth),
            TotalDonatedThisMonth: totalDonatedThisMonth,
            Leaders: leaders,
            CurrentUserRank: userRankDto));
    }
}