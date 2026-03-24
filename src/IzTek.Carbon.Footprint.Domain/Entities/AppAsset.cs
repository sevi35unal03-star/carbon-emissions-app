namespace IzTek.Carbon.Footprint.Domain.Entities;

public class AppAsset : BaseAuditableEntity
{
    public string AssetType { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string? Description { get; private set; }

    private AppAsset()
    { }

    public AppAsset(string assetType, string fileName, string? description = null)
    {
        AssetType = assetType;
        FileName = fileName;
        Description = description;
        IsActive = true;
    }

    public void Update(string fileName, string? description = null)
    {
        FileName = fileName;
        Description = description;
    }
}