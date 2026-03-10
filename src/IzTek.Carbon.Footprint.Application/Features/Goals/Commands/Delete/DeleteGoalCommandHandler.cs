namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Delete;

public static class DeleteGoalCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteGoalCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var goal = await context.Goals
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (goal is null)
            return Result.Failure(
                SystemErrorCodes.GoalNotFound, HttpStatusCode.NotFound);

        context.Goals.Remove(goal);
        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}
