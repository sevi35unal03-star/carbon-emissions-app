using IzTek.Carbon.Footprint.Application.Common.Extensions;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Delete;

public static class DeleteGoalCommandHandler
{
    public static async Task<Result> Handle(
        DeleteGoalCommand command,
        IApplicationDbContext context,
        ICacheService cache,
        CancellationToken ct)
    {
        var goal = await context.Goals
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (goal is null)
            return Result.Failure(
                SystemErrorCodes.GoalNotFound, HttpStatusCode.NotFound);

        context.Goals.Remove(goal);
        await context.SaveChangesAsync(ct);

        // Cache invalidation
        await cache.InvalidateAsync(command, ct);

        return Result.Success();
    }
}