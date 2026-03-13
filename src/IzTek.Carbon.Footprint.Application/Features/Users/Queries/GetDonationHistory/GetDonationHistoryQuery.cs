using IzTek.Carbon.Footprint.Application.Common.Constants;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;

public record GetDonationHistoryQuery(Guid UserId) : ICacheableQuery
{
    public string CacheKey => CacheKeys.User.DonationHistory(UserId);
    public TimeSpan? Expiry => TimeSpan.FromMinutes(30);
}