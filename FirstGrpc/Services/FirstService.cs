using Basics;
using Grpc.Core;

namespace FirstGrpc.Services;

public class FirstService : FirstServiceDefinition.FirstServiceDefinitionBase
{
    public override Task<Response> Unary(Request request, ServerCallContext context)
    {
        var response = new Response() { Message = request.Content + " from server" };

        return Task.FromResult(response);
    }

    public override async Task<Response> ClientStream(IAsyncStreamReader<Request> requestStream, ServerCallContext context)
    {
        var response = new Response() { Message = "I got" };

        while (await requestStream.MoveNext())
        {

            var payload = requestStream.Current;

            Console.WriteLine(payload);

            response.Message = payload.ToString();
        }

        return response;
    }

    public override async Task ServerStream(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        var keyValue = context.RequestHeaders.Get("my-key");

        if (keyValue is not null)
        {
            Console.WriteLine($"Key: {keyValue.Key}, value: {keyValue.Value}");
        }

        for (var i = 0; i < 10; i++)
        {
            if (context.CancellationToken.IsCancellationRequested)
                return;

            var response = new Response() { Message = $"{request.Content} -> reply {i + 1}" };
            await responseStream.WriteAsync(response);
            await Task.Delay(100);
        }
    }

    public override async Task BiDirectionalStream(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        var response = new Response();

        while (await requestStream.MoveNext())
        {
            var payload = requestStream.Current;

            response.Message = payload.ToString();

            await responseStream.WriteAsync(response);
        }
    }
}
