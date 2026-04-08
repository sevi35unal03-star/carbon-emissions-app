namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Create;

public class CreateActivityQuestionCommand
{
    public string Text { get; set; } = default!;
    public int DisplayOrder { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan ScheduledTime { get; set; }
    public TimeSpan NotificationTime { get; set; }
    public List<CreateActivityOptionRequest> Options { get; set; } = new();
}

