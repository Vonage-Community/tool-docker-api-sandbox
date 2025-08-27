#region
using System.Text.Json;
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test;

public class HealthCheckTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> applicationWithAllSpecs = TestApplicationFactory<Program>
        .Builder(helper)
        .Build();

    private readonly TestApplicationFactory<Program> applicationWithoutSpecs = TestApplicationFactory<Program>
        .Builder(helper)
        .WithEnvironmentVariable("CLEAR_SPECS", "true")
        .Build();

    [Fact]
    public async Task GetEnvironmentVariable_ShouldReturnNotFound_GivenVariableDoesNotExist()
    {
        var result = await this.applicationWithoutSpecs.CreateClient()
            .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/_/environment/NonExistentVariable"));
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEnvironmentVariable_ShouldReturnOk_GivenVariableExists()
    {
        var result = await this.applicationWithoutSpecs.CreateClient()
            .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/_/environment/CLEAR_SPECS"));
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var value = await result.Content.ReadAsStringAsync();
        value.Should().Be("true");
    }

    [Fact]
    public async Task Health_ShouldReturnEmptyCollection_GivenNoDocumentIsLoaded()
    {
        var result = await this.applicationWithoutSpecs.CreateClient()
            .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/_/health"));
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadAsStringAsync();
        var specs = JsonSerializer.Deserialize<string[]>(content);
        specs.Should().BeEmpty();
    }

    [Fact]
    public async Task Health_ShouldReturnLoadedSpecs_GivenDocumentsAreLoaded()
    {
        var result = await this.applicationWithAllSpecs.CreateClient()
            .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/_/health"));
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadAsStringAsync();
        var specs = JsonSerializer.Deserialize<string[]>(content);
        specs.Should().BeEquivalentTo("Sms", "Application", "Voice", "VerifyV2", "Messages", "SimSwap",
            "IdentityInsights");
    }
}