namespace IzTek.Carbon.Footprint.Application.Common.Options;

public class ServiceEndpoints
{
    public ServiceEndpoint Platform { get; set; } = new();
}

public class ServiceEndpoint
{
    public int RetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
    public string BaseUrl { get; set; } = string.Empty;
}