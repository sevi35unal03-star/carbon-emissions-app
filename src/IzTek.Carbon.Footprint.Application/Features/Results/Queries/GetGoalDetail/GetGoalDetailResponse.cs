namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

public record GetGoalDetailResponse(
    int Month,
    int Year,
    string MonthLabel,          // "Aralık 2023"
    int TargetTreeCount,        // Hedeflenen ağaç sayısı
    List<LeaderboardItemDto> Leaders,
    UserRankDto? CurrentUserRank);

//Bunlar GetMonthlyLeaderboardResponse'daki DTO'larla aynı, Figma'da hedef detay sayfasi yok o yuzden kaldirilacak.
public record LeaderboardItemDto(
    int Rank,
    string FullName,
    int TreeCount,
    bool IsCurrentUser);

public record UserRankDto(
    int Rank,           // 272. sıra
    int TreeCount,

    string Message); // "120 Ağaç ile 272. sıradasınız."