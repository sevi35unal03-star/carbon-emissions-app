namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyActivityDetails;

public static class GetDailyActivityDetailsHandler
{
    public static async Task<Result<DailyActivityDetailsResponse>> Handle(
        GetDailyActivityDetailsQuery request,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        var start = request.Date.Date;
        var end = start.AddDays(1);

        // 1. Kullanıcının o güne ait loglarını getir
        if (userId is null)
            return Result<DailyActivityDetailsResponse>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);
        var logs = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId &&
                        x.ActivityDate >= start &&
                        x.ActivityDate < end)
            .Select(x => new DailyActivityDetailDto(
                x.ActivityQuestionId,
    x.ActivityQuestion.Text,
    x.ActivityOption.Text,
    x.TotalCarbonScore,
    x.ActivityDate)) // ← düzeltildi
            .ToListAsync(ct);

        if (!logs.Any())
            return Result<DailyActivityDetailsResponse>.Failure(
                SystemErrorCodes.ActivityNotFound, HttpStatusCode.NotFound);

        // 2. Toplam skoru hesapla
        var totalScore = logs.Sum(x => x.Score);

        return Result<DailyActivityDetailsResponse>.Success(
            new DailyActivityDetailsResponse(
                start,
                totalScore,
                logs));
    }
}