using IzTek.Carbon.Footprint.Application.Common.Constants;
using IzTek.Carbon.Footprint.Application.Common.Extensions;

namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;

public static class GetUsefulInformationsQueryHandler
{
    public static async Task<Result<List<GetUsefulInformationsResponse>>> Handle(
        GetUsefulInformationsQuery query,
        IApplicationDbContext context,
        ICacheService cache,
        CancellationToken ct)
    {
        var cacheKey = CacheKeys.UsefulInformation.List;

        // Cache check
        if (await cache.GetCachedResultAsync<List<GetUsefulInformationsResponse>>(cacheKey, ct) is { } hit)
            return hit;

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

        var result = Result<List<GetUsefulInformationsResponse>>.Success(informations);

        // 1 saat cache — statik içerik
        await cache.SetCachedResultAsync(cacheKey, result, TimeSpan.FromHours(1), ct);

        return result;
    }
}