using OpenMatchFunction.Services;
using OpenMatchFunction.Configurations;
using OpenMatchFunction.Interceptors;
using OpenMatchFunction.OM;
using OpenMatchFunction.Exceptions;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateSlimBuilder(args);	 // .net 8 + AOT supported

builder.Host.UseSerilog();
builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddGrpcService()
    .AddHealthChecksService();
//.AddSwaggerService();
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("Grpc.Net.Client");
        metrics.AddMeter("Grpc.AspNetCore.Server");
        
        /*metrics.AddOtlpExporter(o =>
        {
            var endpoint = builder.Configuration["OpenTelemety:Endpoint"] ?? "http://localhost";
            o.Endpoint = new Uri(endpoint);
        });*/
    });

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
    .AddInterceptor<ExceptionInterceptor>()
    .AddStandardResilienceHandler();


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