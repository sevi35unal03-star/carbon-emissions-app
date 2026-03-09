using Iztek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using Microsoft.AspNetCore.Identity;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService)
{
    public async Task<Result<GetUserProfileResponse>> HandleAsync(
        GetUserProfileQuery request,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        if (string.IsNullOrEmpty(userId))
            return Result<GetUserProfileResponse>.Failure(
                "Oturum açmanız gerekiyor.",
                HttpStatusCode.Unauthorized);

        // ✅ Guid → string dönüşümü
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null || user.IsDeleted)
            return Result<GetUserProfileResponse>.Failure(
                "Kullanıcı bulunamadı.",
                HttpStatusCode.NotFound);

        var response = new GetUserProfileResponse(
            user.IdentityNumber ?? string.Empty,
            user.Name,
            user.Surname,
            user.BirthDate);

        return Result<GetUserProfileResponse>.Success(response);
    }
}