namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPreviousAnswers;

public record PreviousAnswerItemDto(
    string QuestionText,
    string AnswerText,
    double Score,
    DateTime Date);

public record PreviousAnswerGroupDto(
    DateTime Date,
    List<PreviousAnswerItemDto> Answers);