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

        // Önce UpdateAsync — anonimleştirilmiş veriyi kaydet
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            // Hata mesajını logla — sebebi görelim
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(SystemErrorCodes.BadRequest, errors, HttpStatusCode.BadRequest);
        }

        await userManager.RemovePasswordAsync(user);
        await userManager.UpdateSecurityStampAsync(user);
        await tokenService.RevokeAllUserTokensAsync(user.Id, "Account deleted");
        await bus.PublishAsync(new UserDeletedDomainEvent(user.Id, DateTime.UtcNow));

        return Result.Success();
    }
}