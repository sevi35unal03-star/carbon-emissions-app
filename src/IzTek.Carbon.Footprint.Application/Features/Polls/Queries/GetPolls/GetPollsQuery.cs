namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetPolls;
public record GetPollsQuery();

public record PollSummaryResponse(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt,
    int QuestionCount);