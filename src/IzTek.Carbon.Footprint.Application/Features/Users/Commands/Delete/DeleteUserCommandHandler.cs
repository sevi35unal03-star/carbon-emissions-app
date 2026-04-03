using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.AspNetCore.Identity;
using Wolverine;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Delete;

public class DeleteUserCommandHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    IMessageBus bus,
    ITokenService tokenService)
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || string.IsNullOrEmpty(currentUserService.UserId.ToString()))
            return Result.Failure(SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(currentUserService.UserId.ToString());

        if (user == null || user.IsDeleted)
            return Result.Failure(SystemErrorCodes.UserNotFound, HttpStatusCode.NotFound);

        user.Delete();

        await userManager.UpdateSecurityStampAsync(user);
        await userManager.RemovePasswordAsync(user);

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);

        await bus.PublishAsync(new UserDeletedDomainEvent(user.Id, DateTime.UtcNow));
        await tokenService.RevokeAllUserTokensAsync(user.Id, "Account deleted");
        await userManager.UpdateSecurityStampAsync(user);

        return Result.Success();
    }
}