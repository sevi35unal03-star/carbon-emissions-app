namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetAllPollResults;

public record GetAllPollResultsQuery(Guid PollSetId, int Month, int Year);

public record PollResultSummaryDto(
    Guid UserId,
    string UserName,
    double TotalScore,
    int TreeCount);