using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Wolverine;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Admin için tüm kullanıcıların bugünkü karbon ayak izi özetini, 
    /// aktivite sayılarını ve bağışlanan ağaç sayılarını listeler.
    /// </summary>
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyResultsAsync([FromQuery] GetUserDailyResultsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<UserDailyResultResponse>>>(query));

    /// <summary>
    /// Admin için belirli bir kullanıcının, belirli bir aydaki anket detaylarını 
    /// (sorular, cevaplar ve skorlar) getirir.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: userId, month, year, userName
    /// </remarks>
    [HttpGet("poll-details")]
    public async Task<IActionResult> GetPollDetailsAsync([FromQuery] GetUserPollDetailQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<UserPollDetailResponse>>(query));
}