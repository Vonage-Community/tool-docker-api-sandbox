#region
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Products.Address;

public class AddressTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application =
        TestApplicationFactory<Program>.Builder(helper).Build();

    [Fact]
    public async Task GetAddresses()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/addresses/")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAddress()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/addresses/c49f3586-9b3b-458b-89fc-3c8beb58865c")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteAddress()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v1/addresses/c49f3586-9b3b-458b-89fc-3c8beb58865c")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateAddress()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v1/addresses/")
            .WithAuthorizationHeader("Basic")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Address/Files/CreateAddress.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}