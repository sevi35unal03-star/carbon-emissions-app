using Iztek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Create;
using Iztek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Update;
using Iztek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;
using IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands;
using IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Delete;
using IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Queries.GetList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Wolverine;

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
    public async Task<IActionResult> CreateAsync(CreateUsefulInformationsCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Admin mevcut bir bilgiyi günceller (Başlık, İçerik vb.).
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAsync(UpdateUsefulInformationsCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Admin bir bilgiyi siler.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(new DeleteUsefulInformationsCommand(id)));
}