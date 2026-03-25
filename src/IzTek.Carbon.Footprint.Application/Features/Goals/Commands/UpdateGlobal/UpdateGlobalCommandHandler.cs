namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;

public static class UpdateGlobalGoalCommandHandler
{
    public static async Task<Result<GlobalGoalResponse>> Handle(
        UpdateGlobalGoalCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var goal = await context.Goals
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (goal is null)
            return Result<GlobalGoalResponse>.Failure(
                SystemErrorCodes.GoalNotFound, HttpStatusCode.NotFound);

        goal.UpdateTarget(command.TargetTreeCount);
        await context.SaveChangesAsync(ct);

        return Result<GlobalGoalResponse>.Success(
            new GlobalGoalResponse(goal.Id, goal.Month, goal.Year, goal.TargetTreeCount));
    }
}