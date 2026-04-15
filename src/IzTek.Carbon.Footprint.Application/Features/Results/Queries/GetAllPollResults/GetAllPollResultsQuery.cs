namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetAllPollResults;

public class GetAllPollResultsQuery
{
    public Guid PollSetId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public record PollResultSummaryDto(
    Guid UserId,
    string UserName,
    double TotalScore,
    int TreeCount);