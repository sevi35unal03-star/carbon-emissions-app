namespace Iztek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;

public record GetUsefulInformationsQuery : ICacheableQuery
{
    public string CacheKey => "useful-informations";
    public TimeSpan? Expiry => TimeSpan.FromHours(24); 
}