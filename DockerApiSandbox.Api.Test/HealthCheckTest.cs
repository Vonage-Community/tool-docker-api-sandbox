#region
using System.Net;
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test;

public class HealthCheckTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper)
        .WithEnvironmentVariable("CLEAR_SPECS", "true")
        .Build();

    [Fact]
    public async Task Health_ShouldReturnOk()
    {
        var result = await this.application.CreateClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, "/_/health"));
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetEnvironmentVariable_ShouldReturnNotFound_GivenVariableDoesNotExist()
    {
        var result = await this.application.CreateClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, "/_/environment/NonExistentVariable"));
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEnvironmentVariable_ShouldReturnOk_GivenVariableExists()
    {
        var result = await this.application.CreateClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, "/_/environment/CLEAR_SPECS"));
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var value = await result.Content.ReadAsStringAsync();
        value.Should().Be("true");
    }
}