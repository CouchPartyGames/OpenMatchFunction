using OpenMatchFunction;
using OpenMatchFunction.Services;
using OpenMatchFunction.Configurations;
using OpenMatchFunction.Interceptors;
using OpenMatchFunction.OM;
using OpenMatchFunction.Exceptions;
using OpenMatchFunction.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;



var builder = WebApplication.CreateSlimBuilder(args);	 // .net 8 + AOT supported



var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("OpenMatchFunction", null, "1.0.0")
    .AddTelemetrySdk();

    // Configuration
    // Enable Experimental Support for gRPC Traces
builder.Configuration.AddInMemoryCollection(
    new Dictionary<string, string?>
    {
        ["OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_ENABLE_GRPC_INSTRUMENTATION"] = "true",
    });
builder.Services.Configure<OpenMatchOptions>(
    builder.Configuration.GetSection(OpenMatchOptions.SectionName));
builder.Services.Configure<OpenTelemetryOptions>(
    builder.Configuration.GetSection(OpenTelemetryOptions.SectionName));

    // Logging
builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(opts =>
{
    opts.SetResourceBuilder(resourceBuilder);
    opts.IncludeScopes = true;
    opts.IncludeFormattedMessage = true;
    opts.AddOtlpExporter(export =>
    {
        export.Endpoint = new Uri("http://localhost:4317");
        export.Protocol = OtlpExportProtocol.Grpc;
    });
});



builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddGrpcService()
    .AddHealthChecksService();

    // Clients
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

    // Observability (OpenTelemetry Traces + Metrics)
builder.Services.AddObservability(builder.Configuration, resourceBuilder);

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