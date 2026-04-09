namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;

public static class GetDonationHistoryQueryHandler
{
    /// <summary>
    /// pointspent: kullanıcının agacı oluştururken harcadığı puan neden kullanılıyor?
    /// </summary>
    public static async Task<Result<GetDonationHistoryResponse>> Handle(
        GetDonationHistoryQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return Result<GetDonationHistoryResponse>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var donations = await context.TreeDonations
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.DonationDate)
            .Select(x => new DonationDto(x.TreeCount, x.PointsSpent, x.DonationDate))
            .ToListAsync(ct);

        return Result<GetDonationHistoryResponse>.Success(new GetDonationHistoryResponse(
            donations.Sum(x => x.TreeCount),
            donations));
    }
}