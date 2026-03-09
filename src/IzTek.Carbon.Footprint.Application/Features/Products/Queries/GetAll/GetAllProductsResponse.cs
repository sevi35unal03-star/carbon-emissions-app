namespace IzTek.Carbon.Footprint.Application.Features.Products.Queries.GetAll;

public class GetAllProductsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CategoryType Category { get; set; }
}