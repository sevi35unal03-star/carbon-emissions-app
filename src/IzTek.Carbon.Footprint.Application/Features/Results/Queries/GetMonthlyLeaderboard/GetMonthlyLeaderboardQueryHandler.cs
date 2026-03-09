using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public static class GetMonthlyLeaderboardQueryHandler
{
    public static async Task<Result<GetMonthlyLeaderboardResponse>> HandleAsync(
        GetMonthlyLeaderboardQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        // 1. TreeDefinition'dan hedef ağaç sayısını al
        var treeDef = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        var pointPerTree = treeDef is not null && treeDef.PointUnit > 0
            ? treeDef.PointUnit / treeDef.TreeCount
            : 1;

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

        // 3. Liderlik listesi
        var leaders = rankings
            .Select((x, index) => new LeaderboardItemDto(
                Rank: index + 1,
                FullName: x.FullName,
                TreeCount: x.TreeCount))
            .ToList();

        // 4. Giriş yapan kullanıcının sırası
        var currentUserId = Guid.Parse(currentUser.UserId);
        var userRank = rankings
            .Select((x, index) => new { x.UserId, x.TreeCount, Rank = index + 1 })
            .FirstOrDefault(x => x.UserId == currentUserId);

        var userRankDto = userRank is not null
            ? new UserRankDto(
                Rank: userRank.Rank,
                TreeCount: userRank.TreeCount,
                Message: $"{userRank.TreeCount} Ağaç ile {userRank.Rank}. sıradasınız.")
            : null;

        // 5. Hedef ağaç sayısı (TreeDefinition'dan)
        var targetTreeCount = treeDef?.TreeCount ?? 0;

        return Result<GetMonthlyLeaderboardResponse>.Success(new GetMonthlyLeaderboardResponse(
            targetTreeCount,
            leaders,
            userRankDto));
    }
}