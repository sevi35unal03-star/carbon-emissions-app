namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

public record GetMonthlyLeaderboardResponse(
    int TargetTreeCount,        // Hedeflenen ağaç sayısı
    List<LeaderboardItemDto> Leaders,
    UserRankDto? CurrentUserRank); // Giriş yapan kullanıcının sırası

public record LeaderboardItemDto(
    int Rank,
    string FullName,
    int TreeCount);

public record UserRankDto(
    int Rank,
    int TreeCount,
    string Message); // "120 Ağaç ile 272. sıradasınız"