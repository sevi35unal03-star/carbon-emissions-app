

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

public record GetMonthlyPollResponse(
    Guid PollSetId,
    string Name,
    string Description,
    List<PollQuestionResponse> Questions) // Buranın null gelme ihtimaline karşı:
{
    // Questions null ise boş liste ata
    public List<PollQuestionResponse> Questions { get; init; } = Questions ?? new();
}