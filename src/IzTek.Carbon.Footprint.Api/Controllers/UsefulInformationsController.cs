using IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Update;
using IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;
using IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Delete;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/useful-informations")]
public class UsefulInformationsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Tüm kullanıcılar faydalı bilgileri listeleyebilir.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<GetUsefulInformationsResponse>>>(new GetUsefulInformationsQuery()));

    /// <summary>
    /// Admin yeni bir faydalı bilgi içeriği oluşturur.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUsefulInformationsCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Admin mevcut bir bilgiyi günceller (Başlık, İçerik vb.).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateUsefulInformationsCommand command)
    {     command.Id = id;
        return CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
    }

    /// <summary>
    /// Admin bir bilgiyi siler.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(new DeleteUsefulInformationsCommand(id)));
}