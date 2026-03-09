namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetPreviousGoals;

public record GetPreviousGoalsResponse(
    int CurrentTargetTreeCount,         // Güncel hedeflenen ağaç
    List<PreviousGoalDto> CompletedGoals);

public record PreviousGoalDto(
    int Month,
    int Year,
    int TreeCount,
    string Label); // "Aralık 2023"