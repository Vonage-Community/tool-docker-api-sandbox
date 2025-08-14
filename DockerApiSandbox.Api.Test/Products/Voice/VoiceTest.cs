#region
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Products.Voice;

public class VoiceTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application =
        TestApplicationFactory<Program>.Builder(helper)
            .OverrideVoiceSpec(Path.GetFullPath("Products/Voice/Files/voice_local_v2.json"))
            .Build();

    [Fact]
    public async Task CreateCall_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v1/calls")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Voice/Files/CreateCall.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
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
}