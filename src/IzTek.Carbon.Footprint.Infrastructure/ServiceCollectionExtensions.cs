namespace IzTek.Carbon.Footprint.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder ConfigureInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        var serviceEndpoints = builder.Configuration.GetSection("ServiceEndpoints").Get<ServiceEndpoints>()!;

        // MinIO File Storage Configuration
        builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection("FileStorage"));
        var fileStorageOptions = builder.Configuration.GetSection("FileStorage").Get<FileStorageOptions>();

        if (fileStorageOptions is not null && !string.IsNullOrEmpty(fileStorageOptions.Endpoint))
        {
            builder.Services.AddMinio(configureClient => configureClient
                .WithEndpoint(fileStorageOptions.Endpoint)
                .WithCredentials(fileStorageOptions.AccessKey, fileStorageOptions.SecretKey)
                .WithSSL(fileStorageOptions.UseSSL)
                .Build());
        }

        builder.Services.AddHttpClient("token").AddStandardResilienceHandler();

        builder.Services.AddHttpClient("platform", cfg =>
        {
            cfg.BaseAddress = new Uri(serviceEndpoints.Platform.BaseUrl);
            cfg.Timeout = TimeSpan.FromSeconds(serviceEndpoints.Platform.TimeoutSeconds);
        }).AddHttpMessageHandler<PlatformClientCredentialTokenHandler>().AddStandardResilienceHandler();

        builder.Services.AddHttpClient("example-1", cfg =>
        {
            cfg.BaseAddress = new Uri("http://localhost");
        }).ConfigureResilience();

        // ✅ Netgsm SMS client
        builder.Services.AddHttpClient("netgsm", cfg =>
        {
            cfg.BaseAddress = new Uri("https://api.netgsm.com.tr");
            cfg.Timeout = TimeSpan.FromSeconds(10);
        });

        builder.Services.ConfigureServices(builder.Configuration);

        return builder;
    }

    public static IServiceCollection ConfigureServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<PlatformClientCredentialTokenHandler>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFileStorageService, MinioFileStorageService>();

        // Development'ta mock, production'da gerçek
        var useMock = configuration.GetValue<bool>("UseMockPlatformService");
        if (useMock)
            services.AddScoped<IPlatformService, MockPlatformService>();
        else
            services.AddScoped<IPlatformService, PlatformService>();

        return services;
    }

    public static IHttpClientBuilder ConfigureResilience(this IHttpClientBuilder builder)
    {
        builder.AddResilienceHandler("default", cfg =>
        {
            cfg.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions()
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 8,
                BreakDuration = TimeSpan.FromSeconds(30),
                ShouldHandle = static args => ValueTask.FromResult(args is
                {
                    Outcome.Result.StatusCode:
                    HttpStatusCode.RequestTimeout or
                    HttpStatusCode.TooManyRequests
                })
            });
            cfg.AddRetry(new HttpRetryStrategyOptions()
            {
                MaxRetryAttempts = 4,
                Delay = TimeSpan.FromSeconds(3),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>()
                .HandleResult(response => !response.IsSuccessStatusCode)
            });

            cfg.AddTimeout(TimeSpan.FromSeconds(10));

            cfg.AddFallback(new FallbackStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>()
                .HandleResult(response => !response.IsSuccessStatusCode),

                FallbackAction = args =>
                {
                    Console.WriteLine("All retries failed. Sending alert email...");
                    //await SendFailureEmailAsync();
                    return Outcome.FromResultAsValueTask(
                        new HttpResponseMessage(HttpStatusCode.InternalServerError));
                }
            });
        });

        return builder;
    }
}