using IzTek.Carbon.Footprint.Application.Common.Constants;
using IzTek.Carbon.Footprint.Application.Common.Extensions;

namespace IzTek.Carbon.Footprint.Application.Features.Assets.Queries.GetAssets;

public static class GetAssetsQueryHandler
{
    public static async Task<Result<AssetsResponse>> Handle(
        GetAssetsQuery query,
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        ICacheService cache,
        CancellationToken ct)
    {
        var cacheKey = CacheKeys.Assets.All;

        // Cache check
        if (await cache.GetCachedResultAsync<AssetsResponse>(cacheKey, ct) is { } hit)
            return hit;

        var assets = await context.AppAssets
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .ToListAsync(ct);

        var urls = new Dictionary<string, string>();
        foreach (var asset in assets)
        {
            var urlResult = await fileStorage.GetPresignedUrlAsync(
                fileName: asset.FileName,
                expirationMinutes: 60 * 24,
                bucket: "assets");

            if (urlResult.IsSuccessful)
                urls[asset.AssetType] = urlResult.Data;
        }

        var result = Result<AssetsResponse>.Success(new AssetsResponse(
            HomeBackground: urls.GetValueOrDefault(AssetType.HomeBackground),
            HomeHero: urls.GetValueOrDefault(AssetType.HomeHero),
            HomeTreeIcon: urls.GetValueOrDefault(AssetType.HomeTreeIcon),
            CarbonCalculate: urls.GetValueOrDefault(AssetType.CarbonCalculate),
            AppLogo: urls.GetValueOrDefault(AssetType.AppLogo),
            Leaderboard: urls.GetValueOrDefault(AssetType.Leaderboard)));

        // 23 saat cache — presigned URL 24 saat geçerli, 1 saat tampon
        await cache.SetCachedResultAsync(cacheKey, result, TimeSpan.FromHours(23), ct);

        return result;
    }
}