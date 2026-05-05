namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetMonthlyActivities;

public record GetMonthlyActivitiesQuery(int Year, int Month) { }

public record MonthlyActivityResponse(
    double TotalMonthlyScore,
    List<DailyScoreDto> DailyScores);

public record DailyScoreDto(
    DateTime Date,
    double TotalScore);