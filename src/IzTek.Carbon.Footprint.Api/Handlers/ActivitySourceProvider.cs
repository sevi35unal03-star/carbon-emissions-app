namespace IzTek.Carbon.Footprint.Api;

/// <summary>
/// OpenTelemetry ActivitySource yönetimi için static class
/// </summary>
public static class ActivitySourceProvider
{
    private static string? _serviceName;
    private static string? _serviceVersion;
    private static ActivitySource? _activitySource;

    /// <summary>
    /// Global ActivitySource instance'ı
    /// </summary>
    public static ActivitySource Current
    {
        get
        {
            if (_activitySource == null)
            {
                throw new InvalidOperationException(
                    "ActivitySource henüz initialize edilmedi. AddOpenTelemetry() metodunu çağırın.");
            }
            return _activitySource;
        }
    }

    /// <summary>
    /// Service bilgilerini set eder ve ActivitySource'u oluşturur
    /// </summary>
    internal static void Initialize(string serviceName, string? serviceVersion = null)
    {
        _serviceName = serviceName;
        _serviceVersion = serviceVersion ?? GetAssemblyVersion();

        // Önceki ActivitySource'u dispose et
        _activitySource?.Dispose();

        _activitySource = new ActivitySource(_serviceName, _serviceVersion);
    }

    /// <summary>
    /// Assembly version'ını otomatik olarak alır
    /// </summary>
    private static string GetAssemblyVersion()
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        return assembly.GetName().Version?.ToString() ?? "1.0.0";
    }

    /// <summary>
    /// Service name'i döner
    /// </summary>
    public static string ServiceName => _serviceName ?? "UnknownService";

    /// <summary>
    /// Service version'ını döner
    /// </summary>
    public static string ServiceVersion => _serviceVersion ?? "1.0.0";

    /// <summary>
    /// ActivitySource'u temizle (test senaryoları için)
    /// </summary>
    internal static void Reset()
    {
        _activitySource?.Dispose();
        _activitySource = null;
        _serviceName = null;
        _serviceVersion = null;
    }
}