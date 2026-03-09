using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetPreviousGoals;
using System.Globalization;

public class GetPreviousGoalsQueryHandler
{
    public async Task<Result<GetPreviousGoalsResponse>> HandleAsync(
        GetPreviousGoalsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Güncel hedef
        var treeDef = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        // 2. Kullanıcının geçmiş poll sonuçları
        var previousGoals = await context.UserPollResults
            .AsNoTracking()
            .Where(x => x.UserId == query.UserId)
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .Select(x => new PreviousGoalDto(
                x.Month,
                x.Year,
                x.TreeCount,
                new DateTime(x.Year, x.Month, 1)
                    .ToString("MMMM yyyy", new CultureInfo("tr-TR"))))
            .ToListAsync(ct);

        return Result<GetPreviousGoalsResponse>.Success(new GetPreviousGoalsResponse(
            treeDef?.TreeCount ?? 0,
            previousGoals));
    }
}
