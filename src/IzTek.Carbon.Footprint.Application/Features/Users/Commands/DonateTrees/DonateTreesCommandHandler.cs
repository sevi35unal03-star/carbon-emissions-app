using IzTek.Carbon.Footprint.Application.Common.Extensions;
using IzTek.Carbon.Footprint.Domain.Common.Exceptions;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;

public static class DonateTreesCommandHandler
{
    public static async Task<Result<DonateTreesResponse>> Handle(
        DonateTreesCommand command,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ICacheService cache,
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

        if (command.PointsToSpend <= 0)
            return Result<DonateTreesResponse>.Failure(
                SystemErrorCodes.InvalidParameter, HttpStatusCode.BadRequest);

        if (command.PointsToSpend > user.TotalPoints)
            return Result<DonateTreesResponse>.Failure(
                SystemErrorCodes.InsufficientPoints, HttpStatusCode.BadRequest);

        var pointsSpent = command.PointsToSpend;
        var treeCount = (int)treeDef.CalculateTreeCount(pointsSpent);

        try
        {
            user.DonatePoints(pointsSpent, treeCount);
        }
        catch (DomainException)
        {
            return Result<DonateTreesResponse>.Failure(
                SystemErrorCodes.InsufficientPoints, HttpStatusCode.BadRequest);
        }

        var donation = new TreeDonation(user.Id, treeCount, pointsSpent);
        await context.TreeDonations.AddAsync(donation, ct);
        await context.SaveChangesAsync(ct);

        // Cache invalidation
        await cache.InvalidateAsync(command, ct);

        return Result<DonateTreesResponse>.Success(
            new DonateTreesResponse(treeCount, user.DonatedTreeCount));
    }
}