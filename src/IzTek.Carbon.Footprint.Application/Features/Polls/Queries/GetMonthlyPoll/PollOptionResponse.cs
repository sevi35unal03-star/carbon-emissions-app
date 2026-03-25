namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

public class PollOptionResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Message { get; set; } // ← eklendi
    public double CarbonValue { get; set; }
    public Guid? NextPollQuestionId { get; set; }
}