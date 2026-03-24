using IzTek.Carbon.Footprint.Application.Common.Constants;
using IzTek.Carbon.Footprint.Application.Common.Extensions;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;

public static class GetDonationHistoryQueryHandler
{
    public static async Task<Result<GetDonationHistoryResponse>> Handle(
        GetDonationHistoryQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ICacheService cache,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return Result<GetDonationHistoryResponse>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var cacheKey = CacheKeys.User.DonationHistory(userId.Value);

        // Cache check
        if (await cache.GetCachedResultAsync<GetDonationHistoryResponse>(cacheKey, ct) is { } hit)
            return hit;

        var donations = await context.TreeDonations
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.DonationDate)
            .Select(x => new DonationDto(x.TreeCount, x.PointsSpent, x.DonationDate))
            .ToListAsync(ct);

        var result = Result<GetDonationHistoryResponse>.Success(new GetDonationHistoryResponse(
            donations.Sum(x => x.TreeCount),
            donations));

        // Cache set
        await cache.SetCachedResultAsync(cacheKey, result, TimeSpan.FromMinutes(30), ct);

        return result;
    }
}