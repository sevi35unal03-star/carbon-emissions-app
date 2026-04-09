using IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.SetTreeDefinition;
using IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.UpdateScoringSettings;
using IzTek.Carbon.Footprint.Application.Features.Definitions.Queries;


namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/v1/definitions")]
public class DefinitionsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    [HttpGet("tree")]
    public async Task<IActionResult> GetTreeDefinitionAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetTreeDefinitionResponse>>(new GetTreeDefinitionQuery()));

    [HttpPut("tree")]
    public async Task<IActionResult> SetTreeDefinitionAsync([FromBody] SetTreeDefinitionCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<SetTreeDefinitionResponse>>(command));

    [HttpGet("scoring-settings")]
    public async Task<IActionResult> GetScoringSettingsAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<List<GetScoringSettingsResponse>>>(new GetScoringSettingsQuery()));

    [HttpPut("scoring-settings")]
    public async Task<IActionResult> UpdateScoringSettingsAsync([FromBody] UpdateScoringSettingsCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result>(command));
}