namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPreviousAnswers;

public record GetPreviousAnswersQuery;

public static class GetPreviousAnswersHandler
{
    public static async Task<Result<List<PreviousAnswerGroupDto>>> Handle(
    GetPreviousAnswersQuery query,
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        if (userId is null)
            return Result<List<PreviousAnswerGroupDto>>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var logs = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.ActivityDate)
            .Select(x => new PreviousAnswerItemDto(
                x.ActivityQuestion.Text,
                x.ActivityOption.Text,
                x.TotalCarbonScore,
                x.ActivityDate))
            .ToListAsync(ct);

        if (!logs.Any())
            return Result<List<PreviousAnswerGroupDto>>.Failure(
                SystemErrorCodes.PreviousAnswersNotFound, HttpStatusCode.NotFound);

        var grouped = logs
            .GroupBy(x => x.Date.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => new PreviousAnswerGroupDto(
                Date: g.Key,
                Answers: g.ToList()))
            .ToList();

        return Result<List<PreviousAnswerGroupDto>>.Success(grouped);
    }
}