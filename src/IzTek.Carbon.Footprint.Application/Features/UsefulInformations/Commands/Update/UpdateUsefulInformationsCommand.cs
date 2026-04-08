

namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Update;

public class UpdateUsefulInformationsCommand
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public int DisplayOrder { get; set; }
}


