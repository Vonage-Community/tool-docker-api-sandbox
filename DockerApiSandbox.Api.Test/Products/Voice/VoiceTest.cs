using System.Net;

namespace DockerApiSandbox.Api.Test.Products.Voice;

public class VoiceTest
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder().Build();
    
    [Fact]
    public async Task GetCalls_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/calls")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetCall_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/calls/CALL-123")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}