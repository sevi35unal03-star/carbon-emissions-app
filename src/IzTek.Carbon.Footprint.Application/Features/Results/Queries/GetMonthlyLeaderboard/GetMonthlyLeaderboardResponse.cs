using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public record GetMonthlyLeaderboardResponse(
    int YearlyTargetTreeCount,
    int MonthlyTargetTreeCount,
    int RemainingTreeCount,
    int TotalDonatedThisMonth,// ← "120.000 ağaç kaldı"
    List<LeaderboardItemDto> Leaders,
    UserRankDto? CurrentUserRank);