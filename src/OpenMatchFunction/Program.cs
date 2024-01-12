using System.Data.SqlTypes;
using OpenMatchFunction.Services;
using OpenMatchFunction.Configurations;
using OpenMatchFunction.Interceptors;
using OpenMatchFunction.OM;
using Microsoft.Extensions.Http.Resilience;
using OpenMatchFunction.Exceptions;
using OpenMatchFunction.Services;
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