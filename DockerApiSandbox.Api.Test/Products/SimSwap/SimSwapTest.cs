using Xunit.Abstractions;

namespace DockerApiSandbox.Api.Test.Products.SimSwap;

public class SimSwapTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper).Build();
    
    [Fact]
    public async Task Check_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/camara/sim-swap/v040/check")
            .WithAuthorizationHeader("oauth2")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/SimSwap/Files/Check.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task RetrieveDate_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/camara/sim-swap/v040/retrieve-date")
            .WithAuthorizationHeader("oauth2")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/SimSwap/Files/RetrieveDate.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}