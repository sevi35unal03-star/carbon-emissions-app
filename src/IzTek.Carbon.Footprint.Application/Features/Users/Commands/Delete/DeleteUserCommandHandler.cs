using Iztek.Carbon.Footprint.Application.Features.Users.Commands.DeleteUser;
using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.AspNetCore.Identity;
using Wolverine;

namespace Iztek.Carbon.Footprint.Application.Features.Users.Commands.Delete;

public class DeleteUserCommandHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    IMessageBus bus) 
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || string.IsNullOrEmpty(currentUserService.UserId))
            return Result.Failure("Unauthorized", HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(currentUserService.UserId);

        if (user == null || user.IsDeleted)
            return Result.Failure("UserNotFound", HttpStatusCode.NotFound);

        user.IsDeleted = true;
        user.DeletedDate = DateTime.UtcNow;

        await userManager.UpdateSecurityStampAsync(user);

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return Result.Failure(result.Errors.First().Code, HttpStatusCode.BadRequest);

        await bus.PublishAsync(new UserDeletedDomainEvent(user.Id, DateTime.UtcNow));

        return Result.Success();
    }
}