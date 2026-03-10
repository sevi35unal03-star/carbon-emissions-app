namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPreviousAnswers;

public record GetPreviousAnswersQuery;

public class GetPreviousAnswersHandler
{
    public async Task<Result<List<PreviousAnswersResponse>>> HandleAsync(
        GetPreviousAnswersQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        var logs = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId) 
            .OrderByDescending(x => x.ActivityDate)
            .Select(x => new PreviousAnswersResponse(
                x.ActivityQuestion.Text,   // ✅ ActivityOption.Question → ActivityQuestion
                x.ActivityOption.Text,
                x.TotalCarbonScore,        // ✅ EarnedScore → TotalCarbonScore
                x.ActivityDate))
            .ToListAsync(ct);

        if (!logs.Any())
            return Result<List<PreviousAnswersResponse>>.Failure(
                SystemErrorCodes.PreviousAnswersNotFound, HttpStatusCode.NotFound);

        return Result<List<PreviousAnswersResponse>>.Success(logs);
    }
}