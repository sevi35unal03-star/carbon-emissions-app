namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;

public static class CreateGoalCommandHandler
{
    public static async Task<Result<CreateGoalResponse>> HandleAsync(
        CreateGoalCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // Aynı ay/yıl için hedef var mı?
        var exists = await context.Goals
            .AnyAsync(x => x.Month == command.Month && x.Year == command.Year, ct);

        if (exists)
            return Result<CreateGoalResponse>.Failure(
                SystemErrorCodes.GoalAlreadyExists, HttpStatusCode.Conflict);

        var goal = new Goal(command.Month, command.Year, command.TargetTreeCount);
        await context.Goals.AddAsync(goal, ct);
        await context.SaveChangesAsync(ct);

        return Result<CreateGoalResponse>.Success(
            new CreateGoalResponse(goal.Id, goal.Month, goal.Year, goal.TargetTreeCount));
    }
}