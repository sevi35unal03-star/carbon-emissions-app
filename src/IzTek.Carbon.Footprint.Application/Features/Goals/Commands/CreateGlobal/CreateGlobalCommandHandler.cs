namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.CreateGlobal;

public static class CreateGlobalGoalCommandHandler
{
    public static async Task<Result<GlobalGoalResponse>> Handle(
        CreateGlobalGoalCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var exists = await context.Goals
            .AnyAsync(x => x.Month == command.Month
                        && x.Year == command.Year, ct);

        if (exists)
            return Result<GlobalGoalResponse>.Failure(
                SystemErrorCodes.GoalAlreadyExists, HttpStatusCode.Conflict);

        var goal = new Goal(command.Month, command.Year, command.TargetTreeCount);
        await context.Goals.AddAsync(goal, ct);
        await context.SaveChangesAsync(ct);

        return Result<GlobalGoalResponse>.Success(
            new GlobalGoalResponse(goal.Id, goal.Month, goal.Year, goal.TargetTreeCount));
    }
}