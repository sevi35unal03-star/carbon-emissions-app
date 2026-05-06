namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyActivityDetails;

public record DailyActivityDetailsResponse(
    DateTime Date,
    double TotalScore,
    List<DailyActivityDetailDto> Activities);

public record DailyActivityDetailDto(
    Guid ActivityQuestionId,
    string QuestionText,
    string SelectedOptionText,
    double Score,
    DateTime CreatedAt);