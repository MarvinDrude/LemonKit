
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog.Sinks.OpenTelemetry;
using Serilog;

namespace LemonKit.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Add Observability
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceName"></param>
    /// <param name="enableDefaultLocalOtlp"></param>
    /// <param name="observerInizialize"></param>
    /// <param name="metricsOptions"></param>
    /// <param name="setSampler"></param>
    /// <param name="tracesOptions">WithTracing</param>
    /// <param name="loggerProviderOptions">ConfigureOpenTelemetryLoggerProvider - For adding exporters</param>
    /// <param name="meterProviderOptions">ConfigureOpenTelemetryMeterProvider - For adding exporters</param>
    /// <param name="tracerProviderOptions">ConfigureOpenTelemetryTracerProvider - For adding exporters</param>
    /// <returns></returns>
    public static IServiceCollection AddCoreObservability(
        this IServiceCollection services,
        string serviceName,
        bool enableDefaultLocalOtlp = true,
        Action? observerInizialize = default,
        Action<MeterProviderBuilder>? metricsOptions = default,
        Action<TracerProviderBuilder>? setSampler = default,
        Action<TracerProviderBuilder>? tracesOptions = default,
        Action<LoggerProviderBuilder>? loggerProviderOptions = default,
        Action<MeterProviderBuilder>? meterProviderOptions = default,
        Action<TracerProviderBuilder>? tracerProviderOptions = default)
    {
        observerInizialize?.Invoke();

        var otel = services.AddOpenTelemetry();

        otel.ConfigureResource(resource =>
        {
            resource.AddService(
                serviceName,
                autoGenerateServiceInstanceId: false,
                serviceInstanceId: serviceName);
        });

        otel.WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel");

            metrics.AddMeter([..ObserveContainer.MeterNames]);
            metricsOptions?.Invoke(metrics);
        });

        otel.WithTracing(tracing =>
        {
            if(setSampler is not null)
            {
                setSampler.Invoke(tracing);
            }
            else
            {
                tracing.SetSampler<AlwaysOnSampler>();
            }
            tracing.AddHttpClientInstrumentation();

            tracing.AddSource([..ObserveContainer.ActivitySourceNames]);
            tracesOptions?.Invoke(tracing);
        });

        services.ConfigureOpenTelemetryLoggerProvider(logger =>
        {
            if(enableDefaultLocalOtlp)
            {
                logger.AddOtlpExporter();
            }
            loggerProviderOptions?.Invoke(logger);
        });

        services.ConfigureOpenTelemetryMeterProvider(meter =>
        {
            if(enableDefaultLocalOtlp)
            {
                meter.AddOtlpExporter();
            }
            meterProviderOptions?.Invoke(meter);
        });

        services.ConfigureOpenTelemetryTracerProvider(traces =>
        {
            if(enableDefaultLocalOtlp)
            {
                traces.AddOtlpExporter();
            }
            tracerProviderOptions?.Invoke(traces);
        });

        services.AddMetrics(builder =>
        {

        });
        return services;
    }

    public static void AddSerilogObservability(
        string serviceName,
        IServiceProvider provider,
        LoggerConfiguration config,
        Action<BatchedOpenTelemetrySinkOptions> onOptions)
    {
        config.WriteTo.OpenTelemetry(options =>
        {
            //options.Endpoint = "http://127.0.0.1:4318";
            //options.Protocol = OtlpProtocol.Grpc;
            options.ResourceAttributes = new Dictionary<string, object>()
            {
                ["service.name"] = serviceName
            };

            onOptions.Invoke(options);
        });
    }
}
