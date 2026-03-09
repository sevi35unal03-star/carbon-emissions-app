namespace IzTek.Carbon.Footprint.Api.Options;

/// <summary>
/// OpenTelemetry konfigürasyon seçenekleri (basit versiyon)
/// </summary>
public class OpenTelemetryOptions
{
    /// <summary>
    /// Servis adı (zorunlu)
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Servis versiyonu (opsiyonel, otomatik assembly version alınır)
    /// </summary>
    public string? ServiceVersion { get; set; }

    /// <summary>
    /// Ek ActivitySource isimleri
    /// </summary>
    public List<string> AdditionalSources { get; set; } = [];

    /// <summary>
    /// OTLP endpoint (varsayılan: http://localhost:4318)
    /// </summary>
    public string? OtlpEndpoint { get; set; }

    /// <summary>
    /// Console exporter aktif mi?
    /// </summary>
    public bool EnableConsoleExporter { get; set; }

    /// <summary>
    /// Metrics toplansın mı?
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Logging aktif mi?
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Console logging aktif mi? (development için)
    /// </summary>
    public bool EnableConsoleLogging { get; set; } = false;

    /// <summary>
    /// Minimum log level
    /// </summary>
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Metrics export interval (ms)
    /// </summary>
    public int MetricsExportIntervalMs { get; set; } = 10000;

    /// <summary>
    /// Exception stack trace dahil edilsin mi?
    /// </summary>
    public bool IncludeStackTrace { get; set; } = false;

    /// <summary>
    /// Correlation ID otomatik propagate edilsin mi?
    /// </summary>
    public bool PropagateCorrelationId { get; set; } = true;

    /// <summary>
    /// İgnore edilecek path pattern'ları
    /// </summary>
    public List<string> IgnorePatterns { get; set; } =
    [
        "/health", "/metrics", "/favicon.ico", "/_cluster/health"
    ];

    /// <summary>
    /// Dahil edilecek path pattern'ları
    /// </summary>
    public List<string> IncludePatterns { get; set; } =
    [
        "/api/", "/connect/"
    ];

    /// <summary>
    /// HTTP client için ignore pattern'ları
    /// </summary>
    public List<string> HttpClientIgnorePatterns { get; set; } =
    [
        ":9200", "/health", "/metrics", "/_cluster/health"
    ];

    /// <summary>
    /// Custom header mapping (HeaderName -> TagName)
    /// </summary>
    public Dictionary<string, string> CustomHeaders { get; set; } = new()
    {
        { "X-Tenant-Id", "tenant.id" },
        { "X-Correlation-Id", "correlation.id" }
    };

    /// <summary>
    /// Log filtering ayarları (Namespace -> LogLevel)
    /// </summary>
    public Dictionary<string, LogLevel> LogFilters { get; set; } = new()
    {
        { "System", LogLevel.Error },
        { "Microsoft", LogLevel.Error },
        { "Microsoft.AspNetCore", LogLevel.Error },
        { "Microsoft.AspNetCore.Mvc", LogLevel.Warning },
        { "Microsoft.AspNetCore.Routing", LogLevel.Warning },
        { "Microsoft.AspNetCore.Hosting", LogLevel.Warning },
        { "Microsoft.AspNetCore.StaticFiles", LogLevel.Warning },
        { "Microsoft.AspNetCore.Authorization", LogLevel.Warning },
        { "Microsoft.AspNetCore.Authentication", LogLevel.Warning },
        { "Microsoft.EntityFrameworkCore", LogLevel.Error },
        { "Microsoft.EntityFrameworkCore.Database", LogLevel.Error },
        { "Microsoft.EntityFrameworkCore.Storage", LogLevel.Critical },
        { "Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Critical }
    };

    /// <summary>
    /// Batch processor ayarları
    /// </summary>
    public BatchProcessorSettings BatchProcessor { get; set; } = new();

    /// <summary>
    /// Ek resource attribute'ları
    /// </summary>
    public Dictionary<string, object> ResourceAttributes { get; set; } = [];

    /// <summary>
    /// Sampling ratio (0.0 - 1.0)
    /// </summary>
    public double SamplingRatio { get; set; } = 1.0;
}

/// <summary>
/// Batch processor ayarları
/// </summary>
public class BatchProcessorSettings
{
    public int MaxQueueSize { get; set; } = 2048;
    public int ScheduledDelayMs { get; set; } = 5000;
    public int MaxExportBatchSize { get; set; } = 512;
    public int ExporterTimeoutMs { get; set; } = 30000;
}
