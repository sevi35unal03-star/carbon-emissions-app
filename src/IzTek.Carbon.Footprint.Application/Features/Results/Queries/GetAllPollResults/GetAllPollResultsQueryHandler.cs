namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetAllPollResults;

public static class GetAllPollResultsQueryHandler
{
    public static async Task<Result<List<PollResultSummaryDto>>> Handle(
    GetAllPollResultsQuery query,
    IApplicationDbContext context,
    CancellationToken ct)
    {
        Console.WriteLine($"=== PollSetId: {query.PollSetId}, Month: {query.Month}, Year: {query.Year}");

        var results = await context.UserPollResults
            .AsNoTracking()
            .Where(x => x.PollSetId == query.PollSetId
                     && x.Month == query.Month
                     && x.Year == query.Year)
            .ToListAsync(ct);

        Console.WriteLine($"=== Bulunan kayıt: {results.Count}");

        return Result<List<PollResultSummaryDto>>.Success(
            results.Select(x => new PollResultSummaryDto(
                x.UserId,
                x.Name + " " + x.Surname,
                x.TotalScore,
                x.TreeCount)).ToList());
    }
}