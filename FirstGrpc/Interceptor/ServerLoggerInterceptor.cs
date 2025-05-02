using Grpc.Core;
using Grpc.Core.Interceptors;

namespace FirstGrpc.Interceptor;

public class ServerLoggerInterceptor : Grpc.Core.Interceptors.Interceptor
{
    private readonly ILogger<ServerLoggerInterceptor> logger;

    public ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger)
    {
        this.logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            logger.LogInformation($"Server intercepting!");
            return await continuation(request, context);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
