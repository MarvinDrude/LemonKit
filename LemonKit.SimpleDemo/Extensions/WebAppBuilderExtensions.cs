namespace LemonKit.SimpleDemo.Extensions;

public static class WebAppBuilderExtensions {

    public static WebApplicationBuilder AddOpenTelemetrySample(this WebApplicationBuilder builder) {

        ObserveContainer.Initialize();

        var services = builder.Services;
        var otel = services.AddOp

    }

}
