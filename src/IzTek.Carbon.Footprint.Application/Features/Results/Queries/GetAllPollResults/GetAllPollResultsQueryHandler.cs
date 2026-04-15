namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetAllPollResults;

public static class GetAllPollResultsQueryHandler
{
    public static async Task<Result<List<PollResultSummaryDto>>> Handle(
     GetAllPollResultsQuery query,
     IApplicationDbContext context,
     CancellationToken ct)
    {
        // Önce tüm kayıtları getir, filtre olmadan
        var all = await context.UserPollResults
            .AsNoTracking()
            .ToListAsync(ct);

        Console.WriteLine($"=== Toplam UserPollResults: {all.Count}");
        Console.WriteLine($"=== Query: PollSetId={query.PollSetId}, Month={query.Month}, Year={query.Year}");

        foreach (var r in all)
        {
            Console.WriteLine($"--- Id={r.Id}, PollSetId={r.PollSetId}, Month={r.Month}, Year={r.Year}, IsCompleted={r.IsCompleted}");
        }

        var results = all
            .Where(x => x.PollSetId == query.PollSetId
                     && x.Month == query.Month
                     && x.Year == query.Year)
            .Select(x => new PollResultSummaryDto(
                x.UserId,
                x.Name + " " + x.Surname,
                x.TotalScore,
                x.TreeCount))
            .ToList();

        Console.WriteLine($"=== Filtrelenmiş: {results.Count}");

        return Result<List<PollResultSummaryDto>>.Success(results);
    }
}