namespace IzTek.Carbon.Footprint.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder, Action<OpenTelemetryOptions> configure)
    {
        var options = new OpenTelemetryOptions();

        configure(options);

        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            throw new ArgumentException("ServiceName zorunludur");
        }

        builder.Services.AddOpenTelemetry(configure);

        builder.AddOpenTelemetryLogging(options);

        return builder;
    }

    public static IHostApplicationBuilder AddOpenTelemetryLogging(this IHostApplicationBuilder builder, OpenTelemetryOptions options)
    {
        if (!options.EnableLogging)
        {
            return builder;
        }

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(options.MinimumLogLevel);

        foreach (var filter in options.LogFilters)
        {
            builder.Logging.AddFilter(filter.Key, filter.Value);
        }

        if (options.EnableConsoleLogging)
        {
            builder.Logging.AddConsole();
        }

        builder.Logging.AddOpenTelemetry(configure =>
        {
            configure.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(
                    serviceName: options.ServiceName,
                    serviceVersion: options.ServiceVersion ?? ActivitySourceProvider.ServiceVersion)
                .AddEnvironmentVariableDetector());

            configure.IncludeScopes = true;
            configure.ParseStateValues = true;
            configure.IncludeFormattedMessage = true;

            if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
            {
                configure.AddProcessor(
                    new BatchLogRecordExportProcessor(
                        new OtlpLogExporter(new OtlpExporterOptions
                        {
                            Endpoint = new Uri(options.OtlpEndpoint)
                        }),
                        maxQueueSize: options.BatchProcessor.MaxQueueSize,
                        scheduledDelayMilliseconds: options.BatchProcessor.ScheduledDelayMs,
                        maxExportBatchSize: options.BatchProcessor.MaxExportBatchSize));
            }

            if (options.EnableConsoleExporter)
            {
                configure.AddConsoleExporter();
            }
        });

        return builder;
    }

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, Action<OpenTelemetryOptions> configure)
    {
        var options = new OpenTelemetryOptions();
        configure(options);

        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            throw new ArgumentException("ServiceName zorunludur");
        }

        ActivitySourceProvider.Initialize(options.ServiceName, options.ServiceVersion);

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: options.ServiceName,
                    serviceVersion: options.ServiceVersion ?? ActivitySourceProvider.ServiceVersion);

                foreach (var attr in options.ResourceAttributes)
                {
                    resource.AddAttributes([new KeyValuePair<string, object>(attr.Key, attr.Value)]);
                }

                resource.AddEnvironmentVariableDetector();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(options.ServiceName);

                foreach (var source in options.AdditionalSources)
                {
                    tracing.AddSource(source);
                }

                tracing.AddNpgsql();
                tracing.AddEntityFrameworkCoreInstrumentation();
                tracing.AddSqlClientInstrumentation(options => options.RecordException = true);

                tracing.AddAspNetCoreInstrumentation(aspNetOptions =>
                {
                    aspNetOptions.RecordException = true;

                    aspNetOptions.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("exception.message", exception.Message);
                        activity.SetTag("exception.type", exception.GetType().Name);

                        if (options.IncludeStackTrace)
                        {
                            activity.SetTag("exception.stacktrace", exception.StackTrace);
                        }

                        activity.SetTag("inner.exception.message", exception.InnerException?.Message);
                    };

                    aspNetOptions.Filter = (context) =>
                    {
                        var path = context.Request.Path.Value;
                        if (string.IsNullOrWhiteSpace(path))
                            return false;

                        foreach (var pattern in options.IgnorePatterns)
                        {
                            if (path.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                                return false;
                        }

                        return options.IncludePatterns.Any(pattern =>
                            path.Contains(pattern, StringComparison.OrdinalIgnoreCase));
                    };

                    aspNetOptions.EnrichWithHttpRequest = (activity, request) =>
                    {
                        foreach (var header in options.CustomHeaders)
                        {
                            if (request.Headers.TryGetValue(header.Key, out var values))
                            {
                                activity.SetTag(header.Value, values.FirstOrDefault());
                            }
                        }
                    };
                });

                tracing.AddHttpClientInstrumentation(httpOptions =>
                {
                    httpOptions.RecordException = true;

                    httpOptions.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("exception.message", exception.Message);
                        activity.SetTag("exception.type", exception.GetType().Name);

                        if (options.IncludeStackTrace)
                        {
                            activity.SetTag("exception.stacktrace", exception.StackTrace);
                        }
                    };

                    httpOptions.EnrichWithHttpRequestMessage = (activity, request) =>
                    {
                        if (activity.ParentId != null && options.PropagateCorrelationId)
                        {
                            request.Headers.Add("X-Correlation-Id", activity.TraceId.ToString());
                        }
                    };

                    httpOptions.FilterHttpRequestMessage = (request) =>
                    {
                        var uri = request.RequestUri?.AbsoluteUri;
                        if (string.IsNullOrWhiteSpace(uri))
                            return false;

                        return !options.HttpClientIgnorePatterns.Any(pattern =>
                            uri.Contains(pattern, StringComparison.OrdinalIgnoreCase));
                    };
                });

                if (options.SamplingRatio < 1.0)
                {
                    tracing.SetSampler(new TraceIdRatioBasedSampler(options.SamplingRatio));
                }

                if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
                {
                    tracing.AddProcessor(
                        new BatchActivityExportProcessor(
                            new OtlpTraceExporter(new OtlpExporterOptions
                            {
                                Endpoint = new Uri(options.OtlpEndpoint)
                            }),
                            maxQueueSize: options.BatchProcessor.MaxQueueSize,
                            scheduledDelayMilliseconds: options.BatchProcessor.ScheduledDelayMs,
                            maxExportBatchSize: options.BatchProcessor.MaxExportBatchSize,
                            exporterTimeoutMilliseconds: options.BatchProcessor.ExporterTimeoutMs));
                }

                if (options.EnableConsoleExporter)
                {
                    tracing.AddConsoleExporter();
                }
            });

        if (options.EnableMetrics)
        {
            services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddNpgsqlInstrumentation();

                    metrics.AddProcessInstrumentation();

                    metrics.AddRuntimeInstrumentation();

                    metrics.AddSqlClientInstrumentation();

                    metrics.AddAspNetCoreInstrumentation();

                    metrics.AddHttpClientInstrumentation();

                    if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
                    {
                        metrics.AddReader(
                            new PeriodicExportingMetricReader(
                                new OtlpMetricExporter(new OtlpExporterOptions
                                {
                                    Endpoint = new Uri(options.OtlpEndpoint)
                                }),
                                exportIntervalMilliseconds: options.MetricsExportIntervalMs));
                    }
                });
        }

        return services;
    }
}