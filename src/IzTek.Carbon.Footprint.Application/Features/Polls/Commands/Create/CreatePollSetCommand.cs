namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public record CreatePollSetCommand(
    string Name,
    int Month,
    int Year
)
{
    public string? Description { get; internal set; }
    public int DisplayOrder { get; internal set; }
}