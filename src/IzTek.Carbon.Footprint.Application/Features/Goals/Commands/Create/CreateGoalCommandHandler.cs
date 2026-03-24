using IzTek.Carbon.Footprint.Application.Common.Extensions;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;

public static class CreateGoalCommandHandler
{
    public static async Task<Result<CreateGoalResponse>> Handle(
        CreateGoalCommand command,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ICacheService cache,
        CancellationToken ct)
    {
        if (currentUser.UserId is null)
            return Result<CreateGoalResponse>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var userId = currentUser.UserId.Value;

        var exists = await context.Goals
            .AnyAsync(x => x.UserId == userId
                        && x.Month == command.Month
                        && x.Year == command.Year, ct);

        if (exists)
            return Result<CreateGoalResponse>.Failure(
                SystemErrorCodes.GoalAlreadyExists, HttpStatusCode.Conflict);

        var goal = new Goal(userId, command.Month, command.Year, command.TargetTreeCount);
        await context.Goals.AddAsync(goal, ct);
        await context.SaveChangesAsync(ct);

        // Cache invalidation
        await cache.InvalidateAsync(command, ct);

        return Result<CreateGoalResponse>.Success(
            new CreateGoalResponse(goal.Id, goal.Month, goal.Year, goal.TargetTreeCount));
    }
}