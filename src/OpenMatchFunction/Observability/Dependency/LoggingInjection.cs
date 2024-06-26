using OpenMatchFunction.Observability.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace OpenMatchFunction.Observability.Dependency;

public static class LoggingInjection
{

    public static ILoggingBuilder AddObservabilityLogging(this ILoggingBuilder loggingBuilder,
        IConfiguration configuration,
        ResourceBuilder resourceBuilder)
    {
        var options = configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Get<OpenTelemetryOptions>();
        
        loggingBuilder.ClearProviders();
        loggingBuilder.AddConsole();
        //loggingBuilder.AddJsonConsole();
        /*
        loggingBuilder.AddOpenTelemetry(opts =>
        {
            opts.SetResourceBuilder(resourceBuilder);
            opts.IncludeScopes = true;
            opts.IncludeFormattedMessage = true;
            opts.AddConsoleExporter();
            
            /*opts.AddOtlpExporter(export =>
            {
                export.Endpoint = new Uri(OpenTelemetryOptions.OtelDefaultHost);
                export.Protocol = OtlpExportProtocol.Grpc;
            });
        });*/
        return loggingBuilder;
    }

}