using IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.SetTreeDefinition;
using IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.UpdateScoringSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Wolverine;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/definitions")]
public class DefinitionsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Bir ağaç dikimi için gereken puan eşiğini ve ağaç birim maliyetlerini tanımlar veya günceller.
    /// Upsert semantiği taşıdığından PUT kullanılır.
    /// </summary>
    [HttpPut("tree")]
    public async Task<IActionResult> SetTreeDefinitionAsync([FromBody] SetTreeDefinitionCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<SetTreeDefinitionResponse>>(command));

    /// <summary>
    /// Karbon ayak izi hesaplama parametrelerini ve aktivite bazlı puanlama ayarlarını günceller.
    /// </summary>
    [HttpPut("scoring-settings")]
    public async Task<IActionResult> UpdateScoringSettingsAsync([FromBody] UpdateScoringSettingsCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<UpdateScoringSettingsResponse>>(command));
}