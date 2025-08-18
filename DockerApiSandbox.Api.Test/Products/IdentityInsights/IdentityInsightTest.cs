using Xunit.Abstractions;

namespace DockerApiSandbox.Api.Test.Products.IdentityInsights;

public class IdentityInsightsTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper).Build();
    
    [Fact]
    public async Task RetrieveFromPhoneNumber_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v0.1/identity-insights")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/IdentityInsights/Files/RetrieveFromPhoneNumber.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}