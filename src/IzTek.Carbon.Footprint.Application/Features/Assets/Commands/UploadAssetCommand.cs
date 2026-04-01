
namespace IzTek.Carbon.Footprint.Application.Features.Assets.Commands;

public record UploadAssetCommand(
    string AssetType,
    Stream FileStream,
    string FileName,
    string ContentType) 
{
}

public class UploadAssetCommandValidator : AbstractValidator<UploadAssetCommand>
{
    private static readonly string[] AllowedTypes = ["image/png", "image/jpeg", "image/webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public UploadAssetCommandValidator()
    {
        RuleFor(x => x.AssetType)
            .Must(AssetType.IsValid)
            .WithMessage($"Geçerli asset tipleri: {string.Join(", ", AssetType.All)}");

        RuleFor(x => x.ContentType)
            .Must(ct => AllowedTypes.Contains(ct))
            .WithMessage("Sadece PNG, JPEG ve WebP formatları desteklenir.");

        RuleFor(x => x.FileStream)
            .Must(s => s.Length <= MaxFileSizeBytes)
            .WithMessage("Dosya boyutu 5 MB'ı geçemez.");
    }
}