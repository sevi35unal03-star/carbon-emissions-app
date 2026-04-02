using Microsoft.Extensions.Caching.Memory;

namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;

public static class GetUsefulInformationsQueryHandler
{
    private const string CacheKey = "usefulinformations";

    public static async Task<Result<List<GetUsefulInformationsResponse>>> Handle(
        GetUsefulInformationsQuery query,
        IApplicationDbContext context,
        IMemoryCache cache,
        CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out List<GetUsefulInformationsResponse>? cached))
            return Result<List<GetUsefulInformationsResponse>>.Success(cached!);

        var informations = await context.UsefulInformations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new GetUsefulInformationsResponse
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                DisplayOrder = x.DisplayOrder
            })
            .ToListAsync(ct);

        cache.Set(CacheKey, informations, TimeSpan.FromDays(60));

        return Result<List<GetUsefulInformationsResponse>>.Success(informations);
    }
}