using Microsoft.AspNetCore.Identity;
using IzTek.Carbon.Footprint.Application.Common.Models;
using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using System.Net;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    IApplicationDbContext context)
{
    public async Task<Result<GetUserProfileResponse>> Handle(
        GetUserProfileQuery request,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        if (userId is null)
            return Result<GetUserProfileResponse>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result<GetUserProfileResponse>.Failure(
                SystemErrorCodes.NotFound, HttpStatusCode.NotFound);

        // Aktif ağaç tanımı - kaç ağaç bağışlanabileceğini hesapla
        var treeDef = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        var availableTreeCount = treeDef is not null
            ? (int)treeDef.CalculateTreeCount(user.TotalPoints)
            : 0;

        return Result<GetUserProfileResponse>.Success(new GetUserProfileResponse(
            identityNumber: user.IdentityNumber ?? string.Empty,
            name: user.Name,
            surname: user.Surname,
            birthDate: user.BirthDate,
            totalPoints: user.TotalPoints,
            donatedTreeCount: user.DonatedTreeCount,
            availableTreeCount: availableTreeCount
        ));
    }
}