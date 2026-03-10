namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

public class PollOptionResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public double CarbonValue { get; set; }
    public Guid? NextPollQuestionId { get; set; }
}; 