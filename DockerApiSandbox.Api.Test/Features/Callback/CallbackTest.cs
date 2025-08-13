#region
using System.Net;
using WireMock.Server;
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Features.Callback;

public class OperationCallbackTest(ITestOutputHelper helper)
{
    [Fact]
    public async Task ShouldSendCallback_WithSpecificServer()
    {
        var callbackServer = WireMockServer.Start();
        var callbackUrl = callbackServer.Url + "/sms/delivery-receipt";
        var application = TestApplicationFactory<Program>.Builder(helper)
            .OverrideSmsSpec(Path.GetFullPath("Features/Callback/Files/spec.json"))
            .WithEnvironmentVariable("SMS_DLR", callbackUrl)
            .WithEnvironmentVariable("CLEAR_SPECS", "true")
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/callback")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        callbackServer.LogEntries.Any(entry => entry.RequestMessage.Url == callbackUrl).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnOk_GivenNoCallbackConfigured()
    {
        var application = TestApplicationFactory<Program>.Builder(helper)
            .OverrideSmsSpec(Path.GetFullPath("Features/Callback/Files/spec.json"))
            .WithEnvironmentVariable("CLEAR_SPECS", "true")
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/callback")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}