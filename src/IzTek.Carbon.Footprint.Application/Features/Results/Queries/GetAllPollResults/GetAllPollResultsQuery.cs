namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetAllPollResults;

public class GetAllPollResultsQuery
{
    public Guid PollSetId { get; init; }
    public int Month { get; init; }
    public int Year { get; init; }

    public GetAllPollResultsQuery(Guid pollSetId, int month, int year)
    {
        PollSetId = pollSetId;
        Month = month;
        Year = year;
    }
}

public record PollResultSummaryDto(
    Guid UserId,
    string UserName,
    double TotalScore,
    int TreeCount);