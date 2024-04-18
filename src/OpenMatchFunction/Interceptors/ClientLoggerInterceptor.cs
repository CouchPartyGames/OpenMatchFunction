namespace OpenMatchFunction.Interceptors;

public sealed class ClientLoggerInterceptor(ILogger<ClientLoggerInterceptor> logger) : Interceptor
{
    private readonly ILogger<ClientLoggerInterceptor> _logger = logger;

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        ClientLoggerInterceptorLog.LogCall(_logger, context.Method.Name);
        return base.BlockingUnaryCall(request, context, continuation);
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        ClientLoggerInterceptorLog.LogCall(_logger, context.Method.Name);
        return base.AsyncUnaryCall(request, context, continuation);
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        ClientLoggerInterceptorLog.LogCall(_logger, context.Method.Name);
        return base.AsyncServerStreamingCall(request, context, continuation);
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        ClientLoggerInterceptorLog.LogCall(_logger, context.Method.Name);
        return base.AsyncClientStreamingCall(context, continuation);
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        ClientLoggerInterceptorLog.LogCall(_logger, context.Method.Name);
        return base.AsyncDuplexStreamingCall(context, continuation);
    }
}

public static partial class ClientLoggerInterceptorLog
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Call `{Method}`")]
    public static partial void LogCall(
        ILogger logger, string method);
}