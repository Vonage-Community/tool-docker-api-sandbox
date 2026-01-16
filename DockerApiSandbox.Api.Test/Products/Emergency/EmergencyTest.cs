#region
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Products.Emergency;

public class EmergencyTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application =
        TestApplicationFactory<Program>.Builder(helper).Build();
    
    [Fact]
    public async Task GetNumber()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/emergency/numbers/15500900000")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task AssignNumber()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Patch)
            .WithUrl("/v1/emergency/numbers/15500900000")
            .WithAuthorizationHeader("Basic")
            .WithJsonBodyFromFile(Path.GetFullPath($"Products/Emergency/Files/PatchNumber.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}