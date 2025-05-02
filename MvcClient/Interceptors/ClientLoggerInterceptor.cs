using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace MvcClient.Interceptors;

public class ClientLoggerInterceptor: Interceptor
{
    private readonly ILogger logger;

    public ClientLoggerInterceptor(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<ClientLoggerInterceptor>();
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {

        try
        {
            logger.LogInformation($" starting the client call of {context.Method.FullName}, {context.Method.Type}");

            return continuation(request, context);
        }
        catch (Exception)
        {

            throw;
        }
        
    }
}
