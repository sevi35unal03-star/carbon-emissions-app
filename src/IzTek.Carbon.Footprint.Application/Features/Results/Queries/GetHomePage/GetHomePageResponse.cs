namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetHomePage;

public record GlobalTargetDto(
    int TargetTreeCount,
    int DonatedTreeCount,
    int RemainingTreeCount,
    double ProgressPercent);

public record MonthlyTargetDto(
    int Month,
    int Year,
    int TargetTreeCount,
    int DonatedTreeCount,
    int RemainingTreeCount,
    double ProgressPercent);

public record HomeLeaderItemDto(
    int Rank,
    string FullName,
    int TreeCount,
    bool IsCurrentUser);

public record HomeUserRankDto(
    int Rank,
    int TreeCount,
    string Message);

public record GetHomePageResponse(
    GlobalTargetDto GlobalTarget,
    MonthlyTargetDto MonthlyTarget,
    List<HomeLeaderItemDto> TopLeaders,

    HomeUserRankDto? CurrentUserRank);  // ← Eklendi