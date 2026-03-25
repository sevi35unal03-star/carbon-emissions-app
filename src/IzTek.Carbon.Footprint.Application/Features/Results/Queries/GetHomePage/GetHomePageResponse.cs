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
    bool HasCompletedPoll,              // ← Flutter buna göre ekran seçer
    GlobalTargetDto? GlobalTarget,      // ← nullable — anketi doldurmamışsa null
    MonthlyTargetDto? MonthlyTarget,    // ← nullable
    List<HomeLeaderItemDto>? TopLeaders, // ← nullable
    HomeUserRankDto? CurrentUserRank);