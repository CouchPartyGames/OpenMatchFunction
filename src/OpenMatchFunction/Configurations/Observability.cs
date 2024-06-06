using Microsoft.Extensions.Options;
using OpenMatchFunction.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenMatchFunction.Configurations;

public static class Observability
{
    public static IServiceCollection AddObservability(this IServiceCollection services, 
        IConfiguration configuration
        /*ILogger logger*/)
    {
        const OtlpExportProtocol otelProtocol = OtlpExportProtocol.Grpc;
        var openTelemetryOptions = configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Get<OpenTelemetryOptions>();
        if (openTelemetryOptions is null)
        {
            //logger.LogError("Null reference for OpenTelemetry options");
            return services;
        }

        services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => resourceBuilder.AddService("OpenMatchFunction", null, "1.0.0"))
            .WithMetrics(opts =>
            {
                //opts.SetResourceBuilder(resourceBuilder);
                opts.AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation();
                
                opts.AddOtlpExporter(export =>
                {
                    export.Endpoint = new Uri(openTelemetryOptions.Endpoint);
                    export.Protocol = otelProtocol;
                });
            })
            .WithTracing(opts =>
            {
                //opts.SetResourceBuilder(resourceBuilder);
                opts.SetSampler(new TraceIdRatioBasedSampler(openTelemetryOptions.SamplingRate));

                opts.AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation();
                
                opts.AddOtlpExporter(export =>
                {
                    export.Endpoint = new Uri(openTelemetryOptions.Endpoint);
                    export.Protocol = otelProtocol;
                });
            });
            services.AddSingleton<OpenMatchFunctionMetrics>();

        return services;
    }
}

