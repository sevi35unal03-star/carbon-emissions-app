using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;

public static class UpdateGlobalGoalCommandHandler
{
    public static async Task<Result<UpdateGoalResponse>> Handle(
        UpdateGlobalGoalCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var goal = await context.Goals
            .FirstOrDefaultAsync(x => x.Id == command.Id
                                   && x.UserId == null, ct);

        if (goal is null)
            return Result<UpdateGoalResponse>.Failure(
                SystemErrorCodes.GoalNotFound, HttpStatusCode.NotFound);

        goal.UpdateTarget(command.TargetTreeCount);
        await context.SaveChangesAsync(ct);

        return Result<UpdateGoalResponse>.Success(
            new UpdateGoalResponse(goal.Id, goal.Month, goal.Year, goal.TargetTreeCount));
    }
}