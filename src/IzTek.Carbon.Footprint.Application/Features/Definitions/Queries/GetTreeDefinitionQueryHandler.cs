namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Queries;

public record GetTreeDefinitionQuery;

public record GetTreeDefinitionResponse(
    double PointUnit,
    int TreeCount,
    int GlobalTargetTreeCount);

public static class GetTreeDefinitionQueryHandler
{
    public static async Task<Result<GetTreeDefinitionResponse>> Handle(
        GetTreeDefinitionQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var definition = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        if (definition is null)
            return Result<GetTreeDefinitionResponse>.Failure(
                SystemErrorCodes.TreeDefinitionNotFound, HttpStatusCode.NotFound);

        return Result<GetTreeDefinitionResponse>.Success(
            new GetTreeDefinitionResponse(
                definition.PointUnit,
                definition.TreeCount,
                definition.GlobalTargetTreeCount));
    }
}