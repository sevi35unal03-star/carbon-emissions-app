namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;

public record GetDonationHistoryQuery(Guid UserId) : ICacheableQuery
{
    public string CacheKey => $"donation-history:{UserId}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(30);
}