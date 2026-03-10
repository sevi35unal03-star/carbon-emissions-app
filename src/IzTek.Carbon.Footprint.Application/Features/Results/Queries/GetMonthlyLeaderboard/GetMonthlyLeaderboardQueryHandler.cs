using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;
namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public static class GetMonthlyLeaderboardQueryHandler
{
    public static async Task<Result<GetMonthlyLeaderboardResponse>> HandleAsync(
        GetMonthlyLeaderboardQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        // 1. Goal tablosundan aylık ve yıllık hedefi al
        var monthlyGoal = await context.Goals
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Month == query.Month && x.Year == query.Year, ct);

        var yearlyTarget = await context.Goals
            .AsNoTracking()
            .Where(x => x.Year == query.Year)
            .SumAsync(x => x.TargetTreeCount, ct);

        var monthlyTarget = monthlyGoal?.TargetTreeCount ?? 0;

        // 2. O aya ait poll sonuçlarından sıralama yap
        var rankings = await context.UserPollResults
     .AsNoTracking()
     .Where(x => x.Month == query.Month && x.Year == query.Year)
     .OrderByDescending(x => x.TreeCount)
     .Select(x => new
     {
         x.UserId,
         x.TreeCount,
         FullName = x.Name + " " + x.Surname
     })
     .ToListAsync(ct);

        // 3. Giriş yapan kullanıcının ID'si
        var currentUserId = currentUser.UserId;

        // 4. Liderlik listesi
        var leaders = rankings
            .Select((x, index) => new LeaderboardItemDto(
                Rank: index + 1,
                FullName: x.FullName,
                TreeCount: x.TreeCount,
                IsCurrentUser: x.UserId == currentUserId))
            .ToList();

        // 5. Giriş yapan kullanıcının sırası
        var userRank = rankings
            .Select((x, index) => new { x.UserId, x.TreeCount, Rank = index + 1 })
            .FirstOrDefault(x => x.UserId == currentUserId);

        var userRankDto = userRank is not null
            ? new UserRankDto(
                Rank: userRank.Rank,
                TreeCount: userRank.TreeCount,
                Message: $"{userRank.TreeCount} Ağaç ile {userRank.Rank}. sıradasınız.")
            : null;

        return Result<GetMonthlyLeaderboardResponse>.Success(new GetMonthlyLeaderboardResponse(
            yearlyTargetTreeCount: yearlyTarget,
            monthlyTargetTreeCount: monthlyTarget,
            leaders: leaders,
            currentUserRank: userRankDto));
    }
}