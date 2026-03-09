namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

public record GetMonthlyLeaderboardResponse(
    int TargetTreeCount,        // Hedeflenen ağaç sayısı
    List<LeaderboardItemDto> Leaders,
    UserRankDto? CurrentUserRank); // Giriş yapan kullanıcının sırası

