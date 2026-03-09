namespace IzTek.Carbon.Footprint.Api.Options;

public class HealthCheckConfigurationOptions
{
    public int MemoryThresholdMB { get; set; } = 1024;
    public string StorageDrive { get; set; } = "C:\\";
    public long StorageMinimumFreeMB { get; set; } = 10_000;
}
