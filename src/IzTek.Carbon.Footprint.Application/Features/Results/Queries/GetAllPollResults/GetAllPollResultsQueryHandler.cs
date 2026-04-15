namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetAllPollResults;

public static class GetAllPollResultsQueryHandler
{
    public static async Task<Result<List<PollResultSummaryDto>>> Handle(
        GetAllPollResultsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var results = await context.UserPollResults
            .AsNoTracking()
            .Where(x => x.PollSetId == query.PollSetId
                     && x.Month == query.Month
                     && x.Year == query.Year
                     && x.IsCompleted)
            .Select(x => new PollResultSummaryDto(
                x.UserId,
                x.Name + " " + x.Surname,
                x.TotalScore,
                x.TreeCount))
            .ToListAsync(ct);

        return Result<List<PollResultSummaryDto>>.Success(results);
    }
}