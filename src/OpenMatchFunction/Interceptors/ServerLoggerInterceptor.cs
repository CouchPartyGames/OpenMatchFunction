namespace OpenMatchFunction.Interceptors;

public sealed class ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, 
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            ServerLoggerInterceptorLog.ServerErrorResponse(logger);
            logger.LogError($"Failure {ex.Message}");
            throw;
        }
    }

    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, 
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        return base.ClientStreamingServerHandler(requestStream, context, continuation);
    }

    public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, 
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
    }

    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream, 
        ServerCallContext context, 
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        
        return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
    }
}


public static partial class ServerLoggerInterceptorLog
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Server failed")]
    public static partial void ServerErrorResponse(
        ILogger logger);
}