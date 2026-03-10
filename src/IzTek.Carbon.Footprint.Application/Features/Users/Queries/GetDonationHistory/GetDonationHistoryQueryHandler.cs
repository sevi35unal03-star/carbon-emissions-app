namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;

public static class GetDonationHistoryQueryHandler
{
    public static async Task<Result<GetDonationHistoryResponse>> HandleAsync(
        GetDonationHistoryQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var donations = await context.TreeDonations
            .AsNoTracking()
            .Where(x => x.UserId == query.UserId)
            .OrderByDescending(x => x.DonationDate)
            .Select(x => new DonationDto(
                x.TreeCount,
                x.PointsSpent,
                x.DonationDate))
            .ToListAsync(ct);

        var totalTreeCount = donations.Sum(x => x.TreeCount);

        return Result<GetDonationHistoryResponse>.Success(
            new GetDonationHistoryResponse(totalTreeCount, donations));
    }
}