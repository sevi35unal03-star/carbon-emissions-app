namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Update;

public record UpdatePollOptionCommand(
    Guid OptionId,
    string Text,
    double Value,
    int DisplayOrder
)
{
    public Guid? NextPollQuestionId { get; internal set; }
}