namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries;

public record ActivityQuestionResponse
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan NotificationTime { get; set; }
    public TimeSpan SendPushNotification {  get; set; }
    public List<ActivityOptionResponse> Options { get; set; } = new List<ActivityOptionResponse>();
};