using OpenMatchFunction.Services;
using OpenMatchFunction.Configurations;
using OpenMatchFunction.Interceptors;
using OpenMatchFunction.OM;
using OpenMatchFunction.Exceptions;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("OpenMatchFunction")
    .AddTelemetrySdk();

var builder = WebApplication.CreateSlimBuilder(args);	 // .net 8 + AOT supported

builder.Host.UseSerilog();
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
            export.Endpoint = new Uri("http://otel");
            export.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
    })
    .WithTracing(opts =>
    {
        opts.SetResourceBuilder(resourceBuilder);
        opts.SetSampler(new TraceIdRatioBasedSampler(0.1f));
        
        opts.AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddGrpcClientInstrumentation();
        
        opts.AddOtlpExporter(export =>
        {
            export.Endpoint = new Uri("http://otel");
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

app.UseSerilogRequestLogging();
app.MapGrpcService<MatchFunctionRunService>();
app.MapGrpcHealthChecksService();

app.Run();

public partial class Program { }