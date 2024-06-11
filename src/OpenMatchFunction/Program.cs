using OpenMatchFunction.Clients.OpenMatchPool;
using OpenMatchFunction.Services;
using OpenMatchFunction.Observability;
using OpenMatchFunction.Observability.Dependency;
using OpenMatchFunction.Utilities;


var builder = WebApplication.CreateSlimBuilder(args);	 // .net 8 + AOT supported

    // Configuration
    // Enable Experimental Support for gRPC Traces (this might not be necessary anymore)
builder.Configuration.AddInMemoryCollection(
    new Dictionary<string, string?>
    {
        ["OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_ENABLE_GRPC_INSTRUMENTATION"] = "true",
    });

    // Add Observability
builder.Logging.AddObservabilityLogging(builder.Configuration, OtelResourceBuilder.ResourceBuilder);
builder.Services.AddObservabilityMetrics(builder.Configuration, OtelResourceBuilder.ResourceBuilder);
builder.Services.AddObservabilityTracing(builder.Configuration, OtelResourceBuilder.ResourceBuilder);

    // Add Clients
builder.Services.AddOpenMatchQueryPoolClient(builder.Configuration);

    // Add Service (gRPC)
builder.Services.AddGrpcService(builder.Configuration);
builder.Services.AddServiceHealthCheck();


var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    /*app.MapGrpcReflectionService();
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MatchFunction V1");
    });*/
}

//new LogRuntime().LogRuntimeInfo();
app.MapGrpcService<MatchFunctionRunService>();
app.MapGrpcHealthChecksService();
app.Run();