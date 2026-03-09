// GetGoalDetailResponse.cs
namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

public record GetGoalDetailResponse(
    int Month,
    int Year,
    string MonthLabel,          // "Aralık 2023"
    int TargetTreeCount,        // Hedeflenen ağaç sayısı
    List<LeaderboardItemDto> Leaders,
    UserRankDto? CurrentUserRank);

public record LeaderboardItemDto(
    int Rank,
    string FullName,
    int TreeCount);

public record UserRankDto(
    int Rank,
    int TreeCount,
    string Message);