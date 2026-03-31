namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

public class PollQuestionResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public List<PollOptionResponse> Options { get; set; } = new();
    public Guid? SelectedOptionId { get; set; } = null; // ← yeni
}