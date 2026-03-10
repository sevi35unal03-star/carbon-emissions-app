using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public record GetMonthlyLeaderboardResponse
{
    public GetMonthlyLeaderboardResponse(
        int yearlyTargetTreeCount,
        int monthlyTargetTreeCount,
        List<LeaderboardItemDto> leaders,
        UserRankDto? currentUserRank)
    {
        YearlyTargetTreeCount = yearlyTargetTreeCount;
        MonthlyTargetTreeCount = monthlyTargetTreeCount;
        Leaders = leaders;
        CurrentUserRank = currentUserRank;
    }

    public int YearlyTargetTreeCount { get; init; }
    public int MonthlyTargetTreeCount { get; init; }
    public List<LeaderboardItemDto> Leaders { get; init; }
    public UserRankDto? CurrentUserRank { get; init; }
}