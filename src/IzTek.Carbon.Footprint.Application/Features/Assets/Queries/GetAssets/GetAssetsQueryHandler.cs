using Microsoft.Extensions.Caching.Memory;

namespace IzTek.Carbon.Footprint.Application.Features.Assets.Queries.GetAssets;

public static class GetAssetsQueryHandler
{
    private const string CacheKey = "assets";

    public static async Task<Result<AssetsResponse>> Handle(
        GetAssetsQuery query,
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        IMemoryCache cache,
        CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out AssetsResponse? cached))
            return Result<AssetsResponse>.Success(cached!);

        var assets = await context.AppAssets
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .ToListAsync(ct);

        //dictionary sözlük tanuımı
        //dictionary kullanarak asset türüne göre url'leri saklıyoruz ve 
        //tek if else bloğu yerine daha temiz ve genişletilebilir bir yapı oluşturuyoruz
        //tek tek verileri kontrol etmiyoz, sadece asset türüne göre url'leri alıyoruz
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

        var response = new AssetsResponse(
            HomeBackground: urls.GetValueOrDefault(AssetType.HomeBackground),
            HomeHero: urls.GetValueOrDefault(AssetType.HomeHero),
            HomeTreeIcon: urls.GetValueOrDefault(AssetType.HomeTreeIcon),
            CarbonCalculate: urls.GetValueOrDefault(AssetType.CarbonCalculate),
            AppLogo: urls.GetValueOrDefault(AssetType.AppLogo),
            Leaderboard: urls.GetValueOrDefault(AssetType.Leaderboard));

        // 23 saat cache — presigned URL 24 saat geçerli, 1 saat tampon
        cache.Set(CacheKey, response, TimeSpan.FromHours(23));

        return Result<AssetsResponse>.Success(response);
    }
}