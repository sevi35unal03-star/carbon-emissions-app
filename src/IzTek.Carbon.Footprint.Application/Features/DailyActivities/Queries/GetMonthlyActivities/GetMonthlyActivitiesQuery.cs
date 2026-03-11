
namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetMonthlyActivities;

public record GetMonthlyActivitiesQuery(int Year, int Month, int Period) { }

public record MonthlyActivityResponse(
    int TotalMonthlyScore,
    int totalPeriodScore,
    List<DailyScoreDto> DailyScores);

public record DailyScoreDto(
    DateTime Date,
    int TotalScore);