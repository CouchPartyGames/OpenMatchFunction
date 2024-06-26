using OpenMatchFunction.Observability.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace OpenMatchFunction.Observability.Dependency;

public static class MetricsInjection
{
    public static IServiceCollection AddObservabilityMetrics(this IServiceCollection services,
        IConfiguration configuration,
        ResourceBuilder resourceBuilder)
    {
        var options = configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Get<OpenTelemetryOptions>();
        
        const OtlpExportProtocol otelProtocol = OtlpExportProtocol.Grpc;
        const string endpoint = OpenTelemetryOptions.OtelDefaultHost;
        
        services.AddOpenTelemetry()
            .WithMetrics(opts =>
            {
                opts.SetResourceBuilder(resourceBuilder);
                opts.AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation();

                opts.AddOtlpExporter(export =>
                {
                    export.Endpoint = new Uri(endpoint);
                    export.Protocol = otelProtocol;
                });
            });

        services.AddSingleton<OtelMetrics>();
        return services;
    }

}