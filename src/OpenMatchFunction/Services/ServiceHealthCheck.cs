namespace OpenMatchFunction.Services;

public static class ServiceHealthCheck
{
    public static IServiceCollection AddServiceHealthCheck(this IServiceCollection services) {
        //services.AddGrpcHealthChecks();
        services.AddGrpcHealthChecks()
            .AddCheck("OpenMatchFunction", () => HealthCheckResult.Healthy(), ["grpc", "live"] );
        
        return services;
    }
    
    
}