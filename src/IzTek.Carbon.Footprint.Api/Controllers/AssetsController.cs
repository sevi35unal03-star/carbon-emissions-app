using IzTek.Carbon.Footprint.Application.Features.Assets.Commands;
using IzTek.Carbon.Footprint.Application.Features.Assets.Queries.GetAssets;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[ApiController]
[Route("api/v1/assets")]
public class AssetsController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    /// <summary>
    /// Tüm statik asset URL'lerini döner.
    /// Flutter uygulama açılışında bir kez çağırır.
    /// Cache: 24 saat.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAssetsAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<AssetsResponse>>(new GetAssetsQuery()));

    /// <summary>
    /// Admin — belirli bir asset görselini günceller.
    /// assetType: HomeHero | HomeTreeIcon | CarbonCalculate | AppLogo | DonationSuccess
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("{assetType}")]
    public async Task<IActionResult> UploadAssetAsync(
        string assetType,
        IFormFile file)
    {
        if (file is null || file.Length == 0)
            return CreateActionResultInstance(
                Result.Failure(SystemErrorCodes.InvalidParameter, HttpStatusCode.BadRequest));

        var command = new UploadAssetCommand(
            AssetType: assetType,
            FileStream: file.OpenReadStream(),
            FileName: file.FileName,
            ContentType: file.ContentType);

        return CreateActionResultInstance(
            await bus.InvokeAsync<Result>(command));
    }
}