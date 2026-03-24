namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

// Podium (#1, #2, #3) için ayrı DTO
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
    int YearlyTargetTreeCount,
    int MonthlyTargetTreeCount,
    int RemainingTreeCount,
    int TotalDonatedThisMonth,
    List<PodiumItemDto> Podium,         // ← YENİ: #1, #2, #3
    List<LeaderboardItemDto> Leaders,   // ← #4 ve sonrası
    UserRankDto? CurrentUserRank);