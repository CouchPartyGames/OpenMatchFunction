namespace OpenMatchFunction.Interceptors;

public sealed class ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger) : Interceptor
{
    public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, 
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        ServerLoggerInterceptorLog.ServerErrorResponse(logger, context.Method, "ServerStreaming");
        return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
    }
}


public static partial class ServerLoggerInterceptorLog
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Server failed {Method}/{Type}")]
    public static partial void ServerErrorResponse(
        ILogger logger, string method, string type);
}