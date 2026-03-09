namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Update;

public record UpdateActivityQuestionRequest(
    Guid Id,
    string Text,
    int DisplayOrder,
    DateTime StartDate,
    DateTime EndDate,
    TimeSpan NotificationTime,
    TimeSpan SendPushNotification,
    List<UpdateActivityOptionRequest> Options);