using Basics;
using Grpc.Core;

namespace FirstGrpc.Services;

public class FirstService : FirstServiceDefinition.FirstServiceDefinitionBase
{
    public override Task<Response> Unary(Request request, ServerCallContext context)
    {
        return base.Unary(request, context);
    }

    public override Task<Response> ClientStream(IAsyncStreamReader<Request> requestStream, ServerCallContext context)
    {
        return base.ClientStream(requestStream, context);
    }

    public override Task ServerStream(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        return base.ServerStream(request, responseStream, context);
    }

    public override Task BiDirectionalStream(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        return base.BiDirectionalStream(requestStream, responseStream, context);
    }
}
