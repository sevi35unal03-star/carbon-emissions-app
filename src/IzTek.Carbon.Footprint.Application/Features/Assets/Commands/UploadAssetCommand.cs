
namespace IzTek.Carbon.Footprint.Application.Features.Assets.Commands;

public record UploadAssetCommand(
    string AssetType,
    Stream FileStream,
    string FileName,
    string ContentType) 
{
}

