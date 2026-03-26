namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.SetTreeDefinition;

public static class SetTreeDefinitionCommandHandler
{
    public static async Task<Result<SetTreeDefinitionResponse>> HandleAsync(
        SetTreeDefinitionCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var definition = await context.TreeDefinitions
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        if (definition is null)
        {
            definition = new TreeDefinition(
                command.PointUnit,
                command.TreeCount,
                command.GlobalTargetTreeCount);
            context.TreeDefinitions.Add(definition);
        }
        else
        {
            definition.Update(
                command.PointUnit,
                command.TreeCount,
                command.GlobalTargetTreeCount);
        }

        await context.SaveChangesAsync(ct);

        return Result<SetTreeDefinitionResponse>.Success(
            new SetTreeDefinitionResponse(
                definition.PointUnit,
                definition.TreeCount,
                definition.GlobalTargetTreeCount));
    }
}