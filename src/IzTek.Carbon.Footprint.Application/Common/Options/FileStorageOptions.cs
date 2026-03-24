namespace IzTek.Carbon.Footprint.Application.Common.Options;

public class FileStorageOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string DefaultBucket { get; set; } = "default";
    public bool UseSSL { get; set; } = false;
    public string AssetsBucket { get; init; } = "assets";
}
