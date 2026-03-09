namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;

public class GetUsefulInformationsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int DisplayOrder { get; set; }
}