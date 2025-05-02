using Basics;
using FirstGrpc.Services;
using FirstGrpc.Tests.Helpers;
using FluentAssertions;

namespace FirstGrpc.Tests;

public class FirstServiceTest
{
    private readonly IFirstService sut;
    public FirstServiceTest()
    {
        sut = new FirstService();
    }

    [Fact]
    public async Task  Unary_Should_Return()
    {
        var expectedResponse = new Response
        {
            Message = "Message from server HostName"
        };
        var request = new Request
        {
            Content = "Message"
        };

        var callContext = FakeServerCallContext.Create();


        var respose = await  sut.Unary(request, callContext);

        respose.Message.Should().Be(expectedResponse.Message);
    }
}