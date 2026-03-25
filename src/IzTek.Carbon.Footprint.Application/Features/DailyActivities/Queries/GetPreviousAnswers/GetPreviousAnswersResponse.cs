namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPreviousAnswers;

public record PreviousAnswersResponse(
    string QuestionText,
    string SelectedOptionText,
    double Score,           // ✅ int → double, GainedScore → Score
    DateTime Date);

public record PreviousAnswerItemDto(
    string QuestionText,
    string AnswerText,
    double Score,
    DateTime Date);

public record PreviousAnswerGroupDto(
    DateTime Date,
    List<PreviousAnswerItemDto> Answers);