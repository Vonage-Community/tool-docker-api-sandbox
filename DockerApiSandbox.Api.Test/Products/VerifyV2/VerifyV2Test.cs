using System.Net;
using Xunit.Abstractions;

namespace DockerApiSandbox.Api.Test.Products.VerifyV2;

public class VerifyV2Test(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper).Build();
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task Cancel_ShouldReturnNoContent(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v2/verify/REQ-123")
            .WithAuthorizationHeader(auth)
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Theory]
    [InlineData("Products/VerifyV2/Files/SendRequest_SilentAuth.json")]
    [InlineData("Products/VerifyV2/Files/SendRequest_SMS.json")]
    [InlineData("Products/VerifyV2/Files/SendRequest_Voice.json")]
    [InlineData("Products/VerifyV2/Files/SendRequest_WhatsApp.json")]
    [InlineData("Products/VerifyV2/Files/SendRequest_Email.json")]
    //[InlineData("Products/VerifyV2/Files/SendRequest_Fallback.json")]
    public async Task SendRequest_ShouldReturnNoContent(string filepath)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v2/verify")
            .WithAuthorizationHeader("bearer")
            .WithJsonBodyFromFile(Path.GetFullPath(filepath))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task NextWorkflow_ShouldReturnOk(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v2/verify/REQ-123/next_workflow")
            .WithAuthorizationHeader(auth)
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task Check_ShouldReturnOk(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v2/verify/REQ-123")
            .WithAuthorizationHeader(auth)
            .WithJsonBodyFromFile(Path.GetFullPath("Products/VerifyV2/Files/Check.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetSilentAuthCompletion_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v2/verify/REQ-123/silent-auth/redirect")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    // Must fix the ref to template
    // [Theory]
    // [InlineData("Bearer")]
    // [InlineData("Basic")]
    // public async Task GetTemplates_ShouldReturnOk(string auth)
    // {
    //     var request = HttpRequestMessageBuilder.Build()
    //         .WithHttpMethod(HttpMethod.Get)
    //         .WithUrl("/v2/verify/templates")
    //         .WithAuthorizationHeader(auth)
    //         .Create();
    //     var response = await this.application.CreateClient().SendAsync(request);
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task GetTemplateFragments_ShouldReturnOk(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v2/verify/templates/TEM-123/template_fragments")
            .WithAuthorizationHeader(auth)
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task GetTemplate_ShouldReturnOk(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v2/verify/templates/TEM-123")
            .WithAuthorizationHeader(auth)
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task GetTemplateFragment_ShouldReturnOk(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v2/verify/templates/TEM-123/template_fragments/FRA-123")
            .WithAuthorizationHeader(auth)
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task DeleteTemplate_ShouldReturnNoContent(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v2/verify/templates/TEM-123")
            .WithAuthorizationHeader(auth)
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task DeleteTemplateFragment_ShouldReturnNoContent(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v2/verify/templates/TEM-123/template_fragments/FRA-123")
            .WithAuthorizationHeader(auth)
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task CreateTemplate_ShouldReturnCreated(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v2/verify/templates")
            .WithAuthorizationHeader(auth)
            .WithJsonBodyFromFile(Path.GetFullPath("Products/VerifyV2/Files/CreateTemplate.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task CreateTemplateFragment_ShouldReturnCreated(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v2/verify/templates/TEM-123/template_fragments")
            .WithAuthorizationHeader(auth)
            .WithJsonBodyFromFile(Path.GetFullPath("Products/VerifyV2/Files/CreateTemplateFragment.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task UpdateTemplate_ShouldReturnCreated(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Patch)
            .WithUrl("/v2/verify/templates/TEM-123")
            .WithAuthorizationHeader(auth)
            .WithJsonBodyFromFile(Path.GetFullPath("Products/VerifyV2/Files/UpdateTemplate.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData("Bearer")]
    [InlineData("Basic")]
    public async Task UpdateTemplateFragment_ShouldReturnCreated(string auth)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Patch)
            .WithUrl("/v2/verify/templates/TEM-123/template_fragments/FRA-123")
            .WithAuthorizationHeader(auth)
            .WithJsonBodyFromFile(Path.GetFullPath("Products/VerifyV2/Files/UpdateTemplateFragment.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}