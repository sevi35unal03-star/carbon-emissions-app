namespace IzTek.Carbon.Footprint.Application.Features.Assets.Queries.GetAssets;

public record GetAssetsQuery;

public record AssetsResponse(
    string? HomeBackground,
    string? HomeHero,//web tasarımında ve mobil uygulamalarda ana sayfanın en üstünde yer alan,
                     //kullanıcıyı karşılayan en dikkat çekici, büyük ve etkileyici görsel bölümdür.
    string? HomeTreeIcon,
    string? CarbonCalculate,
    string? AppLogo,
    string? Leaderboard);