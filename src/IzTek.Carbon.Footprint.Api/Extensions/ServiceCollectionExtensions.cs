namespace IzTek.Carbon.Footprint.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder ConfigureApi(this IHostApplicationBuilder builder)
    {
        builder.Services.ConfigureRateLimiter();

        builder.Services.ConfigureLocalization();

        builder.Services.ConfigureApiVersioning();

        builder.Services.AddHealthCheckConfiguration(builder.Configuration);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        return builder;
    }

    private static IServiceCollection ConfigureRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy("user", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context?.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromSeconds(15),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    }));

            // Auth policy — IP bazlı, 15 dakikada 5 deneme
            options.AddFixedWindowLimiter("auth", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(15);
                opt.PermitLimit = 5;
                opt.QueueLimit = 0;
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                var response = Result.Failure(SystemErrorCodes.TooManyRequests, HttpStatusCode.TooManyRequests);

                await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(response), token);
            };
        });

        return services;
    }

    private static IServiceCollection ConfigureApiVersioning(this IServiceCollection services, int defaultMajor = 1, int defaultMinor = 0)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(defaultMajor, defaultMinor);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
        });

        return services;
    }

    private static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        services.AddLocalization();

        const string defaultCulture = "tr-TR";

        var supportedCultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo(defaultCulture),
        };

        services.Configure<RequestLocalizationOptions>(opts =>
        {
            opts.DefaultRequestCulture = new RequestCulture(defaultCulture);

            opts.SupportedCultures = supportedCultures;
            opts.SupportedUICultures = supportedCultures;
        });

        return services;
    }

    public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
    {
        var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(localizationOptions.Value);

        return app;
    }
}