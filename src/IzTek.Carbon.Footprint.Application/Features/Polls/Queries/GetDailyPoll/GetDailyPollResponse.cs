using Iztek.Carbon.Footprint.Application.Features.Polls.Queries.GetDailyPoll;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetDailyPoll;

public record GetDailyPollResponse(
    Guid PollSetId,
    string Name,
    string Description,
    List<PollQuestionResponse> Questions);