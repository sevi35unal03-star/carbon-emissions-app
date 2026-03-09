namespace IzTek.Carbon.Footprint.Application.Features.Products.Queries.GetAll;

public static class GetAllProductsQueryHandler
{
    public static async Task<Result<List<GetAllProductsResponse>>> HandleAsync(
        GetAllProductsQuery query,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var products = await context.Products.Select(x => new GetAllProductsResponse()
        {
            Id = x.Id,
            Name = x.Name,
            Category = x.Category,
        }).ToListAsync(cancellationToken);

        return Result<List<GetAllProductsResponse>>.Success(products);
    }
}

