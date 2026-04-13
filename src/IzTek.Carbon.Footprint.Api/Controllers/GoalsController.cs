using IzTek.Carbon.Footprint.Application.Features.Goals.Commands;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.CreateGlobal;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.DeleteGlobal;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/goals")]
public class GoalsController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    [HttpPost("global")]
    public async Task<IActionResult> CreateGlobalGoalAsync([FromBody] CreateGlobalGoalCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GlobalGoalResponse>>(command));

    [HttpPut("global/{id:guid}")]
    public async Task<IActionResult> UpdateGlobalGoalAsync(
        Guid id,
        [FromBody] UpdateGlobalGoalCommand command)
    {
        command = command with { Id = id };
        return CreateActionResultInstance(
            await bus.InvokeAsync<Result<GlobalGoalResponse>>(command));
    }

    [HttpDelete("global/{id:guid}")]
    public async Task<IActionResult> DeleteGlobalGoalAsync(Guid id, [FromQuery] int month, [FromQuery] int year)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result>(new DeleteGlobalGoalCommand(id, month, year)));
}