namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;

public static class DonateTreesCommandHandler
{
    public static async Task<Result<DonateTreesResponse>> HandleAsync(
        DonateTreesCommand command,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == currentUser.UserId, ct);

        if (user is null)
            return Result<DonateTreesResponse>.Failure(
                SystemErrorCodes.UserNotFound, HttpStatusCode.NotFound);

        if (user.TotalPoints <= 0)
            return Result<DonateTreesResponse>.Failure(
                SystemErrorCodes.NoPointsToDonat, HttpStatusCode.BadRequest);

        var treeDef = await context.TreeDefinitions
    .FirstOrDefaultAsync(x => x.IsActive, ct);


        if (treeDef is null)
            return Result<DonateTreesResponse>.Failure(
                SystemErrorCodes.TreeDefinitionNotFound, HttpStatusCode.NotFound);

        var treeCount = (int)treeDef.CalculateTreeCount(user.TotalPoints);

        user.DonateAllPoints(treeCount);

        // Bağış geçmişine kaydet
        var donation = new TreeDonation(user.Id, treeCount, user.TotalPoints);
        await context.TreeDonations.AddAsync(donation, ct);

        await context.SaveChangesAsync(ct);

        return Result<DonateTreesResponse>.Success(
            new DonateTreesResponse(treeCount, user.DonatedTreeCount));
    }
}