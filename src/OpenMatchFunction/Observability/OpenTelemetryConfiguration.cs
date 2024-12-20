using OpenMatchFunction;
using OpenMatchFunction.Observability.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenMatchFunction.Observability;

public static class OpenTelemetryConfiguration
{
    public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var openTelemetryOptions = configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Get<OpenTelemetryOptions>();
        
        
        const OtlpExportProtocol otelProtocol = OtlpExportProtocol.Grpc;
        var endpoint = new Uri(OpenTelemetryOptions.OtelDefaultHost);

        services.AddOpenTelemetry()
            .ConfigureResource(configure =>
            {
                configure
                    .AddService(GlobalConsts.ServiceName, null, GlobalConsts.ServiceVersion)
                    .AddTelemetrySdk()
                    //.AddAttributes()
                    .Build();
            })
            .WithMetrics(opts =>
            {
                opts.AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
                opts.AddOtlpExporter(export =>
                {
                    export.Endpoint = endpoint;
                    export.Protocol = otelProtocol;
                });
            })
            .WithTracing(opts =>
            {
                opts
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation();
                opts.AddOtlpExporter(export =>
                {
                    export.Endpoint = endpoint;
                    export.Protocol = otelProtocol;
                });
            })
            .WithLogging(opts =>
            {
                opts.AddConsoleExporter();
                opts.AddOtlpExporter(export =>
                {
                    export.Endpoint = endpoint;
                    export.Protocol = otelProtocol;
                });
            });
        
        return services;
    }
}