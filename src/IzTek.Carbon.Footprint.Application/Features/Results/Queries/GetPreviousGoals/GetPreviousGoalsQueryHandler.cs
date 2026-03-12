using System.Globalization;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetPreviousGoals;

public static class GetPreviousGoalsQueryHandler
{
    public static async Task<Result<GetPreviousGoalsResponse>> HandleAsync(
        GetPreviousGoalsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Güncel ağaç tanımı
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
                new DateTime(x.Year, x.Month, 1, 0, 0, 0, DateTimeKind.Utc) // ✅ DateTimeKind eklendi
                    .ToString("MMMM yyyy", new CultureInfo("tr-TR"))))
            .ToListAsync(ct);

        return Result<GetPreviousGoalsResponse>.Success(new GetPreviousGoalsResponse(
            treeDef?.TreeCount ?? 0,
            previousGoals));
    }
}