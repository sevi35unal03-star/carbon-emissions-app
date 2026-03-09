using IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;

namespace Iztek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;

public static class GetUsefulInformationsQueryHandler
{
    public static async Task<Result<List<GetUsefulInformationsResponse>>> HandleAsync(
        GetUsefulInformationsQuery query,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var informations = await context.UsefulInformations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new GetUsefulInformationsResponse
            {
                Id = x.PollQuestionId,
                Title = x.Title,
                Content = x.Content,
                DisplayOrder = x.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        return Result<List<GetUsefulInformationsResponse>>.Success(informations);
    }
}