#region
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Products.Redact;

public class RedactTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application =
        TestApplicationFactory<Program>.Builder(helper).Build();

    [Fact]
    public async Task RedactTransaction_ShouldReturnNoContent()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v1/redact/transaction")
            .WithAuthorizationHeader("Basic")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Redact/Files/RedactTransaction.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
