namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Update;

public class UpdateActivityQuestionCommand
{
    public Guid Id { get; set; }  
    public string Text { get; set; } = default!;
    public int DisplayOrder { get; set; }
    public TimeSpan ScheduledTime { get; set; }
    public TimeSpan SendPushNootification { get; set; }  
    public List<UpdateActivityOptionRequest> Options { get; set; } = new();
    public DateTime StartDate { get; internal set; }
    public DateTime EndDate { get; internal set; }
    public TimeSpan NotificationTime { get; internal set; }
}

