namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public class CreatePollOptionRequest
{
    public string Text { get; set; } = string.Empty;
    public double Value { get; set; }
    public string? Message { get; set; } 
    public int DisplayOrder { get; set; }
}