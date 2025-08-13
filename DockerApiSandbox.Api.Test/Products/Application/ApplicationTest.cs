using System.Net;
using Xunit.Abstractions;

namespace DockerApiSandbox.Api.Test.Products.Application;

public class ApplicationTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper).Build();

    [Fact]
    public async Task GetApplications_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v2/applications")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetUsers_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/users")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    
    [Fact]
    public async Task GetApplication_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v2/applications/APP-123")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetUser_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/users/USR-123")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
      
    [Fact]
    public async Task DeleteApplication_ShouldReturnNoContent()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v2/applications/APP-123")
            .WithAuthorizationHeader("Basic")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteUser_ShouldReturnNoContent()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v1/users/USR-123")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task CreateApplication_ShouldReturnCreated()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v2/applications")
            .WithAuthorizationHeader("Basic")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Application/Files/CreateApplication.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnCreated()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v1/users")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Application/Files/CreateUser.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task UpdateApplication_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Put)
            .WithUrl("/v2/applications/APP-123")
            .WithAuthorizationHeader("Basic")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Application/Files/UpdateApplication.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task UpdateUser_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Patch)
            .WithUrl("/v1/users/USR-123")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Application/Files/UpdateUser.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}