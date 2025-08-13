#region
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Features.Identification;

public class OperationIdentificationTest(ITestOutputHelper helper)
{
    [Fact]
    public async Task ShouldReturnNotFound_GivenOperationCantBeFound()
    {
        var application = TestApplicationFactory<Program>.Builder(helper)
            .WithEnvironmentVariable("CLEAR_SPECS", "true")
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v2/unknown-product").Create());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturnSuccess_GivenOnlyOverridenSpecIsLoaded()
    {
        var application = TestApplicationFactory<Program>.Builder(helper)
            .OverrideApplicationSpec(Path.GetFullPath("Features/Identification/Files/spec.json"))
            .WithEnvironmentVariable("CLEAR_SPECS", "true")
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/test").Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task ShouldReturnSuccess_GivenOverridenSpecIsLoaded()
    {
        var application = TestApplicationFactory<Program>.Builder(helper)
            .OverrideApplicationSpec(Path.GetFullPath("Features/Identification/Files/spec.json"))
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/test").Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task ShouldReturnSuccess_GivenOtherSpecIsOverriden()
    {
        var application = TestApplicationFactory<Program>.Builder(helper)
            .OverrideSmsSpec(Path.GetFullPath("Features/Identification/Files/spec.json"))
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithAuthorizationHeader("Basic")
            .WithUrl("/v2/applications")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldReturnSuccess_GivenDocumentIsFoundAfterThirdTry()
    {
        var server = WireMockServer.Start();
        server.Given(Request.Create().WithPath("/spec").UsingGet()).InScenario("Retry").WillSetStateTo("SecondTry")
            .RespondWith(Response.Create().WithStatusCode(500));
        server.Given(Request.Create().WithPath("/spec").UsingGet()).InScenario("Retry").WhenStateIs("SecondTry")
            .WillSetStateTo("ThirdTry").RespondWith(Response.Create().WithStatusCode(500));
        server.Given(Request.Create().WithPath("/spec").UsingGet()).InScenario("Retry").WhenStateIs("ThirdTry")
            .RespondWith(Response.Create().WithBodyFromFile(Path.GetFullPath("Features/Identification/Files/spec.json"))
                .WithStatusCode(200));
        var application = TestApplicationFactory<Program>.Builder(helper)
            .OverrideApplicationSpec(server.Url + "/spec")
            .WithEnvironmentVariable("CLEAR_SPECS", "true")
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/test").Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task ShouldReturnNotFound_GivenDocumentIsMissingAfterThirdTry()
    {
        var server = WireMockServer.Start();
        server.Given(Request.Create().WithPath("/spec").UsingGet()).InScenario("Retry").WillSetStateTo("SecondTry")
            .RespondWith(Response.Create().WithStatusCode(500));
        server.Given(Request.Create().WithPath("/spec").UsingGet()).InScenario("Retry").WhenStateIs("SecondTry")
            .WillSetStateTo("ThirdTry").RespondWith(Response.Create().WithStatusCode(500));
        server.Given(Request.Create().WithPath("/spec").UsingGet()).InScenario("Retry").WhenStateIs("ThirdTry")
            .RespondWith(Response.Create().WithStatusCode(500));
        var application = TestApplicationFactory<Program>.Builder(helper)
            .OverrideApplicationSpec(server.Url + "/spec")
            .WithEnvironmentVariable("CLEAR_SPECS", "true")
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/test").Create());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturnNotFound_GivenNoSpecsAreLoaded()
    {
        var application = TestApplicationFactory<Program>.Builder(helper)
            .WithEnvironmentVariable("CLEAR_SPECS", "true")
            .Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/sms/json")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    
    [Fact]
    public async Task ShouldPickBestMatchRoute_GivenMultipleRoutesCanMatch()
    {
        var application = TestApplicationFactory<Program>.Builder(helper)
            .OverrideApplicationSpec(Path.GetFullPath("Features/Identification/Files/spec.json"))
            .WithEnvironmentVariable("CLEAR_SPECS", "true")
            .Build();
        var response = await application.CreateClient()
            .SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Get)
                .WithUrl("/route/template")
                .Create());
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}