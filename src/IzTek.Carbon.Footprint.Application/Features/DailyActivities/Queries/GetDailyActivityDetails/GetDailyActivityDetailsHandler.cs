namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyActivityDetails;

public class GetDailyActivityDetailsHandler
{
    public async Task<Result<DailyActivityDetailsResponse>> HandleAsync(
        GetDailyActivityDetailsQuery request,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        var start = request.Date.Date;
        var end = start.AddDays(1);

        // 1. Kullanıcının o güne ait loglarını getir
        var logs = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.UserId == Guid.Parse(userId.ToString()) &&
                        x.ActivityDate >= start &&
                        x.ActivityDate < end)
            .Select(x => new DailyActivityDetailDto(
                x.ActivityQuestion.Text,
                x.ActivityOption.Text,
                x.TotalCarbonScore,
                x.CreatedAt))
            .ToListAsync(ct);

        if (!logs.Any())
            return Result<DailyActivityDetailsResponse>.Failure(
                "Bu tarihe ait aktivite bulunamadı.",
                HttpStatusCode.NotFound);

        // 2. Toplam skoru hesapla
        var totalScore = logs.Sum(x => x.Score);

        return Result<DailyActivityDetailsResponse>.Success(
            new DailyActivityDetailsResponse(
                start,
                totalScore,
                logs));
    }
}