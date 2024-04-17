namespace OpenMatchFunction.Interceptors;

public class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : Interceptor
{
   private readonly ILogger<ExceptionInterceptor> _logger = logger;

   public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request,
      ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
   {
      return continuation(request, context);
   }
   
   
   /*public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, 
      ServerCallContext context, 
      UnaryServerMethod<TRequest, TResponse> contination)
   {
      try
      {
         return await contination(request, context);
      }
      catch (Exception e)
      {
         
      }
   }*/
}


public static partial class InterceptorErrors
{
   [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Timeout")]
   public static partial void Timeout(ILogger logger);

   [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "gRPC Error")]
   public static partial void RpcError(ILogger logger);
}