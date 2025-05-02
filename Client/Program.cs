// See https://aka.ms/new-console-template for more information
using Basics;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Grpc.Core.Metadata;

Console.WriteLine("Client running");


var factory = new StaticResolverFactory(addr => new[] {

    new BalancerAddress("localhost",5057),
    new BalancerAddress("localhost",5058),

});

var services = new ServiceCollection();
services.AddSingleton<ResolverFactory>(factory);

var options = new GrpcChannelOptions()
{

};

using var channel = GrpcChannel.ForAddress("https://localhost:7275", options);

//var channel = GrpcChannel.ForAddress("static://localhost", new GrpcChannelOptions()
//{

//    Credentials = ChannelCredentials.Insecure,
//    ServiceConfig = new ServiceConfig
//    {
//        LoadBalancingConfigs = { new RoundRobinConfig() },
//    },
//    ServiceProvider = services.BuildServiceProvider(),

//});

var client = new FirstServiceDefinition.FirstServiceDefinitionClient(channel);

try
{
    Unary(client);
    //await ClientStreaming(client);
    //await ServerStreaming(client);
    //await BiDirectionalStreaming(client);
}
catch (Exception e)
{
    Console.WriteLine(e);
}


Console.ReadLine();


void Unary(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    var metadata = new Metadata { { "grpc-accept-encoding", "gzip" } };
    var request = new Request() { Content = "Hello unary" };

    var response = client.Unary(request, headers: metadata, deadline: DateTime.UtcNow.AddSeconds(5));

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
    var cts = new CancellationTokenSource();
    var metadata = new Metadata();
    metadata.Add(new Entry("my-key", "my-value"));
    metadata.Add(new Entry("second-key", "other value"));

    try
    {
        using var streamingcall = client.ServerStream(new Request { Content = "Hello from client" }, headers: metadata);

        await foreach (var response in streamingcall.ResponseStream.ReadAllAsync(cts.Token))
        {
            Console.WriteLine(response.Message);
        }
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {
        Console.WriteLine($"Failed to send {ex.Message}");
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.PermissionDenied)
    {
        Console.WriteLine($"Permission denied {ex.Message}");
    }
    /*
        var myTrailers = streamingcall.GetTrailers();

        foreach (var item in myTrailers)
        {
            Console.WriteLine($"trailer key: {item.Key}, value: {item.Value}");
        }
    */
}
async Task BiDirectionalStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{

    using (var call = client.BiDirectionalStream())
    {

        var request = new Request();

        for (var i = 0; i < 10; i++)
        {

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
