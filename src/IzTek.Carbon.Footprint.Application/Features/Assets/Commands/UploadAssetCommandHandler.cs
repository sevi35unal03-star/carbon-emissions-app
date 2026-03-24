using IzTek.Carbon.Footprint.Application.Common.Extensions;

namespace IzTek.Carbon.Footprint.Application.Features.Assets.Commands;

public static class UploadAssetCommandHandler
{
    public static async Task<Result> Handle(
        UploadAssetCommand command,
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        ICacheService cache,
        CancellationToken ct)
    {
        if (!AssetType.IsValid(command.AssetType))
            return Result.Failure(
                SystemErrorCodes.InvalidParameter, HttpStatusCode.BadRequest);

        var extension = Path.GetExtension(command.FileName);
        var uniqueFileName = $"{command.AssetType.ToLower()}-{Guid.NewGuid()}{extension}";

        var uploadResult = await fileStorage.UploadFileAsync(
            fileStream: command.FileStream,
            fileName: uniqueFileName,
            contentType: command.ContentType,
            bucket: "assets",
            cancellationToken: ct);

        if (!uploadResult.IsSuccessful)
            return Result.Failure(
                SystemErrorCodes.SystemError, HttpStatusCode.InternalServerError);

        var existing = await context.AppAssets
            .FirstOrDefaultAsync(x => x.AssetType == command.AssetType
                                   && !x.IsDeleted, ct);

        if (existing is not null)
            existing.Update(uniqueFileName);
        else
            context.AppAssets.Add(new AppAsset(command.AssetType, uniqueFileName));

        await context.SaveChangesAsync(ct);

        // Cache invalidation
        await cache.InvalidateAsync(command, ct);

        return Result.Success();
    }
}