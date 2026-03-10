namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;

public record GetYearlyGoalsResponse(
    int YearlyTargetTreeCount,
    List<MonthlyGoalDto> MonthlyGoals);

public record MonthlyGoalDto(
    int Month,
    int Year,
    int TargetTreeCount,
    bool IsCompleted);