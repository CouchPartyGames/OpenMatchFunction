using OpenMatchFunction.Interceptors;
using OpenMatchFunction.Options;

namespace OpenMatchFunction.Clients.OpenMatchPool;

public static class ClientPoolInjection
{
    public static IServiceCollection AddOpenMatchQueryPoolClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddGrpcClient<QueryService.QueryServiceClient>(OpenMatchOptions.OpenMatchQuery, o =>
            {
                var host = configuration["OPENMATCH_QUERY_HOST"] ?? OpenMatchOptions.OpenMatchQueryDefaultHost;
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
        
        return services;
    }
}