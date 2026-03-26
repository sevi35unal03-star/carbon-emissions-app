namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Queries;

public record GetScoringSettingsQuery;

public record GetScoringSettingsResponse(
    Guid Id,
    string Key,
    double Value,
    string Category);

public static class GetScoringSettingsQueryHandler
{
    public static async Task<Result<List<GetScoringSettingsResponse>>> Handle(
        GetScoringSettingsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var settings = await context.ScoringSettings
            .AsNoTracking()
            .OrderBy(x => x.Category)
            .Select(x => new GetScoringSettingsResponse(
                x.Id,
                x.Key,
                x.Value,
                x.Category.ToString()))
            .ToListAsync(ct);

        return Result<List<GetScoringSettingsResponse>>.Success(settings);
    }
}