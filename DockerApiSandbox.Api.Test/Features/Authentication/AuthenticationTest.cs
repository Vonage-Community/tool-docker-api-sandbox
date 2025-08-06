#region
using System.Net;
using FluentAssertions;
#endregion

namespace DockerApiSandbox.Api.Test.Features.Authentication;

public class AuthenticationTest
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder()
        .OverrideApplicationSpec(Path.GetFullPath("Features/Authentication/Files/spec.json"))
        .WithEnvironmentVariable("CLEAR_SPECS", "true")
        .Build();

    [Theory]
    [InlineData("Basic")]
    [InlineData("Bearer")]
    public async Task ShouldReturnOk_GivenEndpointHaveNoAuthAndAuthIsProvided(string scheme)
    {
        var request = HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post).WithUrl("/no-auth")
            .WithAuthorizationHeader(scheme)
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ShouldReturnOk_GivenEndpointHaveNoAuth()
    {
        var request = HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post).WithUrl("/no-auth").Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("/basic")]
    [InlineData("/bearer")]
    public async Task ShouldReturnUnauthorized_GivenAuthorizationHeaderIsMissing(string operation)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl(operation).Create());
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("/basic", "Bearer")]
    [InlineData("/bearer", "Basic")]
    public async Task ShouldReturnUnauthorized_GivenAuthorizationHeaderIsNotSupported(string operation,
        string invalidScheme)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl(operation).WithAuthorizationHeader(invalidScheme).Create());
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("/basic", "Basic")]
    [InlineData("/bearer", "Bearer")]
    public async Task ShouldReturnOk_GivenAuthorizationHeaderMatchesRequirement(string validOperation,
        string validScheme)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl(validOperation).WithAuthorizationHeader(validScheme).Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}