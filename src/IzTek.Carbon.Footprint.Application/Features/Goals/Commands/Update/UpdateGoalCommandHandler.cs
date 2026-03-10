namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;

public static class UpdateGoalCommandHandler
{
    public static async Task<Result<UpdateGoalResponse>> HandleAsync(
        UpdateGoalCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var goal = await context.Goals
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (goal is null)
            return Result<UpdateGoalResponse>.Failure(
                SystemErrorCodes.GoalNotFound,
                HttpStatusCode.NotFound);

        goal.UpdateTarget(command.TargetTreeCount);
        await context.SaveChangesAsync(ct);

        return Result<UpdateGoalResponse>.Success(
            new UpdateGoalResponse(goal.Id, goal.Month, goal.Year, goal.TargetTreeCount));
    }
}