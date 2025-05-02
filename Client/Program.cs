// See https://aka.ms/new-console-template for more information
using Basics;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Tasks;

Console.WriteLine("Hello, World!");

var options = new GrpcChannelOptions()
{

};

using var channel = GrpcChannel.ForAddress("https://localhost:7275", options);

var client = new FirstServiceDefinition.FirstServiceDefinitionClient(channel);

//Unary(client);
//await ClientStreaming(client);
//await ServerStreaming(client);
await BiDirectionalStreaming(client);

Console.ReadLine();


void Unary(FirstServiceDefinition.FirstServiceDefinitionClient client)
{

    var request = new Request() { Content = "Hello unary" };

    var response = client.Unary(request);

    Console.WriteLine(response.Message);
}

async Task ClientStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{

    using var call = client.ClientStream();

    for (var i = 0; i < 10; i++)
    {
        await call.RequestStream.WriteAsync(new Request { Content = $" request {i}" });

        await Task.Delay(1000);
    }
    await call.RequestStream.CompleteAsync();
    var response = await call;

    Console.WriteLine(response.Message);
}

async Task ServerStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    using var streamingcall = client.ServerStream(new Request { Content = "Hello from client" });

    await foreach (var item in streamingcall.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine(item.Message);
    }
}

async Task BiDirectionalStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client) {

    using (var call = client.BiDirectionalStream()) {

        var request = new Request();

        for (var i = 0; i < 10; i++) {

            request.Content = $" Request {i}";
            Console.WriteLine($"client - {request.Content}");
            await call.RequestStream.WriteAsync(request);
        }

        while (await call.ResponseStream.MoveNext())
        {
            var message = call.ResponseStream.Current;
            Console.WriteLine($"response - {message}");
        }

        await call.RequestStream.CompleteAsync();
    }

}
