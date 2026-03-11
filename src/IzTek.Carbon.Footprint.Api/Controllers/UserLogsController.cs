using IzTek.Carbon.Footprint.Application.Features.LogsUser.Queries;


namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-logs")]
public class UserLogsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Admin kullanıcıların sistem üzerindeki aktivitelerini (Audit Logs) listeler.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: userId, startDate, endDate, page, pageSize
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetLogsAsync([FromQuery] GetAuditLogsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<PagedResult<GetAuditLogResponse>>(query));
}