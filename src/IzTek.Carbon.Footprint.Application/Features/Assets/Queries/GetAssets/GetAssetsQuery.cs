using IzTek.Carbon.Footprint.Application.Common.Constants;

namespace IzTek.Carbon.Footprint.Application.Features.Assets.Queries.GetAssets;

public record GetAssetsQuery;

public record AssetsResponse(
    string? HomeBackground,
    string? HomeHero,
    string? HomeTreeIcon,
    string? CarbonCalculate,
    string? AppLogo,
    string? Leaderboard);