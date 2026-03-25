using IzTek.Carbon.Footprint.Application.Common.Extensions;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public static class GetMonthlyLeaderboardQueryHandler
{
    public static async Task<Result<GetMonthlyLeaderboardResponse>> Handle(
        GetMonthlyLeaderboardQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var monthlyGoal = await context.Goals
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Month == query.Month && x.Year == query.Year, ct);

        var yearlyTarget = await context.Goals
            .AsNoTracking()
            .Where(x => x.Year == query.Year)
            .SumAsync(x => x.TargetTreeCount, ct);

        var monthlyTarget = monthlyGoal?.TargetTreeCount ?? 0;

        var totalDonatedThisMonth = await context.TreeDonations
            .AsNoTracking()
            .Where(x => x.DonationDate.Month == query.Month
                     && x.DonationDate.Year == query.Year)
            .SumAsync(x => x.TreeCount, ct);

        var donations = await context.TreeDonations
            .AsNoTracking()
            .Where(x => x.DonationDate.Month == query.Month
                     && x.DonationDate.Year == query.Year)
            .GroupBy(x => x.UserId)
            .Select(g => new { UserId = g.Key, TotalTrees = g.Sum(x => x.TreeCount) })
            .ToListAsync(ct);

        var userIds = donations.Select(x => x.UserId).ToList();
        var users = await context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, FullName = u.Name + " " + u.Surname })
            .ToListAsync(ct);

        var rankingsRaw = donations
            .Join(users, d => d.UserId, u => u.Id,
                (d, u) => new { d.UserId, d.TotalTrees, u.FullName })
            .OrderByDescending(x => x.TotalTrees)
            .ToList();

        var currentUserId = currentUser.UserId;

        var podium = rankingsRaw
            .Take(3)
            .Select((x, i) => new PodiumItemDto(i + 1, x.FullName, x.TotalTrees, x.UserId == currentUserId))
            .ToList();

        var leaders = rankingsRaw
            .Skip(3)
            .Select((x, i) => new LeaderboardItemDto(i + 4, x.FullName, x.TotalTrees, x.UserId == currentUserId))
            .ToList();

        var userEntry = rankingsRaw
            .Select((x, i) => new { x.UserId, x.TotalTrees, Rank = i + 1 })
            .FirstOrDefault(x => x.UserId == currentUserId);

        var userRankDto = userEntry is not null
            ? new UserRankDto(userEntry.Rank, userEntry.TotalTrees,
                $"{userEntry.TotalTrees} Ağaç ile {userEntry.Rank}. sıradasınız.")
            : null;

        return Result<GetMonthlyLeaderboardResponse>.Success(new GetMonthlyLeaderboardResponse(
            YearlyTargetTreeCount: yearlyTarget,
            MonthlyTargetTreeCount: monthlyTarget,
            RemainingTreeCount: Math.Max(0, monthlyTarget - totalDonatedThisMonth),
            TotalDonatedThisMonth: totalDonatedThisMonth,
            Podium: podium,
            Leaders: leaders,
            CurrentUserRank: userRankDto));
    }
}