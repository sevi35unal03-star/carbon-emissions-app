namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    [HttpGet]
    [EnableRateLimiting("user")]
    public async Task<IActionResult> GetAllAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<GetAllProductsResponse>>>(new GetAllProductsQuery()));

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateProductCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
}