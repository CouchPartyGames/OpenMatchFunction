using OpenMatchFunction.Services;
using OpenMatchFunction.Configurations;
using OpenMatchFunction.Interceptors;
using OpenMatchFunction.OM;
using OpenMatchFunction.Exceptions;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;


var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("OpenMatchFunction")
    .AddTelemetrySdk();

var builder = WebApplication.CreateSlimBuilder(args);	 // .net 8 + AOT supported

    // Enable Experimental Support for gRPC Traces
builder.Configuration.AddInMemoryCollection(
    new Dictionary<string, string?>
    {
        ["OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_ENABLE_GRPC_INSTRUMENTATION"] = "true",
    });

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(opts =>
{
    opts.SetResourceBuilder(resourceBuilder);
    opts.IncludeScopes = true;
    opts.IncludeFormattedMessage = true;
    opts.AddOtlpExporter(export =>
    {
        export.Endpoint = new Uri("http://localhost:4317");
    });
});
builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddGrpcService()
    .AddHealthChecksService();

builder.Services
    .AddGrpcClient<QueryService.QueryServiceClient>(Constants.OpenMatchQuery, o => {
        var host = builder.Configuration["OPENMATCH_QUERY_HOST"] ?? "https://open-match-query.open-match.svc.cluster.local:50503";
        o.Address = new Uri(host);
    }).ConfigureChannel(o =>
    {
        o.HttpHandler = new SocketsHttpHandler()
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            EnableMultipleHttp2Connections = true
        };
        o.MaxRetryAttempts = 4;
    })
    .AddInterceptor<ClientLoggerInterceptor>()
    .AddStandardResilienceHandler();

builder.Services.AddOpenTelemetry()
    .WithMetrics(opts =>
    {
        opts.SetResourceBuilder(resourceBuilder);
        opts.AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation();
        
        opts.AddOtlpExporter(export =>
        {
            export.Endpoint = new Uri("http://localhost:4317");
            export.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
    })
    .WithTracing(opts =>
    {
        opts.SetResourceBuilder(resourceBuilder);
        opts.SetSampler(new AlwaysOnSampler());
        //opts.SetSampler(new TraceIdRatioBasedSampler(0.1f));
        
        opts.AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddGrpcClientInstrumentation();
        
        opts.AddOtlpExporter(export =>
        {
            export.Endpoint = new Uri("http://localhost:4317");
            export.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
    });


var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    /*app.MapGrpcReflectionService();
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MatchFunction V1");
    });*/
}

app.MapGrpcService<MatchFunctionRunService>();
app.MapGrpcHealthChecksService();

app.Run();

public partial class Program { }