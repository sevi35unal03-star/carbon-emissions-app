namespace IzTek.Carbon.Footprint.Domain.Enums;

public static class AssetType
{
    public const string HomeBackground = "HomeBackground";
    public const string HomeHero = "HomeHero";
    public const string HomeTreeIcon = "HomeTreeIcon";
    public const string CarbonCalculate = "CarbonCalculate";
    public const string AppLogo = "AppLogo";
    public const string Leaderboard = "Leaderboard";

    public static readonly IReadOnlyList<string> All =
    [
        HomeBackground,
        HomeHero,
        HomeTreeIcon,
        CarbonCalculate,
        AppLogo,
        Leaderboard,
    ];

    public static bool IsValid(string assetType)
        => All.Contains(assetType);
}