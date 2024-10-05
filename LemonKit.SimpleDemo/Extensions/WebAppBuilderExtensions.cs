using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace LemonKit.SimpleDemo.Extensions;

public static class WebAppBuilderExtensions {

    public static WebApplicationBuilder AddOpenTelemetrySample(this WebApplicationBuilder builder) {

        ObserveContainer.Initialize();

        var services = builder.Services;
        var otel = services.AddOpenTelemetry();

        otel.ConfigureResource(resource => {
            resource.AddService("DemoApplication");
        });

        otel.WithMetrics(metrics => {

            metrics
                .AddAspNetCoreInstrumentation()
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel");

            metrics.AddMeter([..ObserveContainer.MeterNames]);

        });

        otel.WithTracing(tracing => {

            tracing.SetSampler<AlwaysOnSampler>();
            tracing.AddHttpClientInstrumentation();

            tracing.AddSource([..ObserveContainer.ActivitySourceNames]);

        });

        services.ConfigureOpenTelemetryLoggerProvider(logger
            => logger.AddOtlpExporter());
        services.ConfigureOpenTelemetryMeterProvider(metrics
            => metrics.AddOtlpExporter());
        services.ConfigureOpenTelemetryTracerProvider(tracing
            => tracing.AddOtlpExporter());

        services.AddMetrics();

        return builder;

    }

}
