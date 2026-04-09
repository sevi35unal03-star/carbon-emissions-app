using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using Microsoft.AspNetCore.Identity;
namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.Get;

public static class GetUserPointsAndTreesHandler
{
    public static async Task<Result<GetUserPointsAndTreesResponse>> Handle(
        GetUserPointsAndTreesQuery query,           
        UserManager<User> userManager,
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId is null)
            return Result<GetUserPointsAndTreesResponse>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result<GetUserPointsAndTreesResponse>.Failure(
                SystemErrorCodes.NotFound, HttpStatusCode.NotFound);

        // Aktif TreeDefinition
        var treeDef = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, cancellationToken);

        var availableTreeCount = treeDef is not null
            ? (int)treeDef.CalculateTreeCount(user.TotalPoints)
            : 0;

        var response = new GetUserPointsAndTreesResponse(
            totalPoints: user.TotalPoints,
            donatedTreeCount: user.DonatedTreeCount,
            availableTreeCount: availableTreeCount
        );

        return Result<GetUserPointsAndTreesResponse>.Success(response);
    }
}