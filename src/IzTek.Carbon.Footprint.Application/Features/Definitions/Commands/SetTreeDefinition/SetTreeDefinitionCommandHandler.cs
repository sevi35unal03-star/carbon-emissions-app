namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.SetTreeDefinition;

public class SetTreeDefinitionCommandHandler
{
    private readonly IApplicationDbContext _context;

    public SetTreeDefinitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SetTreeDefinitionResponse>> HandleAsync(
        SetTreeDefinitionCommand command,
        CancellationToken ct)
    {
        var definition = await _context.TreeDefinitions
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        if (definition is null)
        {
            definition = new TreeDefinition(
                command.PointUnit,
                command.TreeCount,
                command.GlobalTargetTreeCount);
            _context.TreeDefinitions.Add(definition);
        }
        else
        {
            definition.Update(
                command.PointUnit,
                command.TreeCount,
                command.GlobalTargetTreeCount);
        }

        await _context.SaveChangesAsync(ct);

        return Result<SetTreeDefinitionResponse>.Success(
            new SetTreeDefinitionResponse(
                definition.PointUnit,
                definition.TreeCount,
                definition.GlobalTargetTreeCount));
    }
}