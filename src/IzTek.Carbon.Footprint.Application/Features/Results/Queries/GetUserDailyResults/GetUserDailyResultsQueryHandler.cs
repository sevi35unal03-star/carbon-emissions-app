namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;

public class GetUserDailyResultsHandler(IApplicationDbContext context)
{
    public async Task<Result<List<UserDailyResultResponse>>> HandleAsync(
        GetUserDailyResultsQuery query,
        CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // 1. Bugünkü aktivite sayısı ve toplam karbon skoru
        var todayActivityQuery = context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.ActivityDate >= today && x.ActivityDate < tomorrow)
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                Count = g.Count(),
                CarbonScore = g.Sum(x => x.TotalCarbonScore)
            });

        // 2. Kullanıcının bağışladığı ağaç sayısı (TreeDefinition üzerinden hesaplama)
        var treeDefinition = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        var pointPerTree = treeDefinition is not null && treeDefinition.PointUnit > 0
            ? treeDefinition.PointUnit / treeDefinition.TreeCount
            : 1;

        // 3. Ana sorgu
        var results = await (
            from u in context.Users.AsNoTracking()

            join activity in todayActivityQuery
                on u.PollQuestionId equals activity.UserId into actJoin
            from activity in actJoin.DefaultIfEmpty()

            select new UserDailyResultResponse
            {
                Id = u.PollQuestionId,
                LastLoginDate = u.LastLoginDate,
                CarbonFootprintScore = activity != null ? activity.CarbonScore : 0,
                DailyActivitiesCount = activity != null ? activity.Count : 0,
                TotalCurrentScore = u.TotalPoints,
                DonatedTreeCount = (int)(u.TotalPoints / pointPerTree),
                EquivalentPoints = u.TotalPoints
            }
        ).ToListAsync(ct);

        return Result.Success(results);
    }
}