#region
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Products.Numbers;

public class NumbersTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application =
        TestApplicationFactory<Program>.Builder(helper).Build();

    [Fact]
    public async Task GetOwnedNumbers_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/account/numbers")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAvailableNumbers_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/number/search")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BuyNumber_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/number/buy")
            .WithAuthorizationHeader("Basic")
            .WithFormBody("country=GB&msisdn=447700900000")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelNumber_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/number/cancel")
            .WithAuthorizationHeader("Basic")
            .WithFormBody("country=GB&msisdn=447700900000")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateNumber_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/number/update")
            .WithAuthorizationHeader("Basic")
            .WithFormBody("country=GB&msisdn=447700900000")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
