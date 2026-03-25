namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public record PodiumItemDto(
    int Rank,
    string FullName,
    int TreeCount,
    bool IsCurrentUser);

public record LeaderboardItemDto(
    int Rank,
    string FullName,
    int TreeCount,
    bool IsCurrentUser);

public record UserRankDto(
    int Rank,
    int TreeCount,
    string Message);

public record GetMonthlyLeaderboardResponse(
    List<PodiumItemDto> Podium,
    List<LeaderboardItemDto> Leaders,
    UserRankDto? CurrentUserRank);