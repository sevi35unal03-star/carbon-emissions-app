using System.Globalization;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

public class GetGoalDetailQueryHandler
{
    public async Task<Result<GetGoalDetailResponse>> HandleAsync(
        GetGoalDetailQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        // 1. O aya ait poll sonuçları var mı kontrol et
        var anyResult = await context.UserPollResults
            .AsNoTracking()
            .AnyAsync(x => x.Month == query.Month && x.Year == query.Year, ct);

        if (!anyResult)
            return (Result<GetGoalDetailResponse>)Result<GetGoalDetailResponse>.Failure(
                "Bu aya ait hedef verisi bulunamadı.",
                HttpStatusCode.NotFound);

        // 2. TreeDefinition'dan hedef ağaç sayısını al
        var treeDef = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        // 3. O aya ait sıralama
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

        // 4. Liderlik listesi
        var leaders = rankings
            .Select((x, index) => new LeaderboardItemDto(
                Rank: index + 1,
                FullName: x.FullName,
                TreeCount: x.TreeCount))
            .ToList();

        // 5. Giriş yapan kullanıcının sırası
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

        // 6. Ay etiketi
        var monthLabel = new DateTime(query.Year, query.Month, 1)
            .ToString("MMMM yyyy", new CultureInfo("tr-TR")); // "Aralık 2023"

        return Result<GetGoalDetailResponse>.Success(new GetGoalDetailResponse(
            Month: query.Month,
            Year: query.Year,
            MonthLabel: monthLabel,
            TargetTreeCount: treeDef?.TreeCount ?? 0,
            Leaders: leaders,
            CurrentUserRank: userRankDto));
    }
}
