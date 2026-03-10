using Iztek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

public record GetMonthlyPollResponse(
    Guid PollSetId,
    string Name,
    string Description,
    List<PollQuestionResponse> Questions);