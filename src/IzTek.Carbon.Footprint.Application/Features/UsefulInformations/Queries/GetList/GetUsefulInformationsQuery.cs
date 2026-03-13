using IzTek.Carbon.Footprint.Application.Common.Constants;

namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;

public record GetUsefulInformationsQuery : ICacheableQuery
{
    public string CacheKey => CacheKeys.UsefulInformation.List;
    public TimeSpan? Expiry => TimeSpan.FromHours(24);
}