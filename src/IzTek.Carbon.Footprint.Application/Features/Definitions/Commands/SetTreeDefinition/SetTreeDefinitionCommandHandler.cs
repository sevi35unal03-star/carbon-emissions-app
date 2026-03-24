using IzTek.Carbon.Footprint.Application.Common.Constants;

namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.SetTreeDefinition;

public class SetTreeDefinitionCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public SetTreeDefinitionCommandHandler(
        IApplicationDbContext context,
        ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
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
            command.GlobalTargetTreeCount);  // ← YENİ
        _context.TreeDefinitions.Add(definition);
    }
    else
    {
        definition.Update(
            command.PointUnit,
            command.TreeCount,
            command.GlobalTargetTreeCount);  // ← YENİ
    }

    await _context.SaveChangesAsync(ct);
    await _cacheService.RemoveAsync(CacheKeys.TreeDefinition.Ratio);
    await _cacheService.RemoveAsync(CacheKeys.HomePage.Prefix);

        return Result<SetTreeDefinitionResponse>.Success(
        new SetTreeDefinitionResponse(
            definition.PointUnit,
            definition.TreeCount,
            definition.GlobalTargetTreeCount));  // ← YENİ
}
}