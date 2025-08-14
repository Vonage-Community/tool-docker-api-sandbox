#region
using System.Net;
using Xunit.Abstractions;
#endregion



namespace DockerApiSandbox.Api.Test.Features.InputValidation;

public class InputValidationTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper)
        .OverrideApplicationSpec(Path.GetFullPath("Features/InputValidation/Files/spec.json"))
        .WithEnvironmentVariable("CLEAR_SPECS", "true")
        .Build();
    
    [Fact]
    public async Task ShouldReturnOk_WithValidPathParameter()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/path/USR-123").Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("/query")]
    [InlineData("/query?param_1=123")]
    [InlineData("/query?param_2=test")]
    [InlineData("/query?order=asc")]
    [InlineData("/query?param_1=123&param_2=123456789&order=desc")]
    public async Task ShouldReturnOk_WithValidQueryParameter(string validOperation)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl(validOperation).Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("/query?param_1=name")]
    [InlineData("/query?order=1")]
    [InlineData("/query?order=middle")]
    public async Task ShouldReturnBadRequest_WithInvalidQueryParameter(string invalidOperation)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl(invalidOperation).Create());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_GivenMissingRequiredBodyParameter()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body")
            .WithJsonBodyFromFile("Features/InputValidation/Files/MissingRequiredBodyParameter.json")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_GivenMissingBody()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_GivenInvalidEnum()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body")
            .WithJsonBodyFromFile("Features/InputValidation/Files/InvalidEnumParameter.json")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_GivenLowerThanMinimum()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body")
            .WithJsonBodyFromFile("Features/InputValidation/Files/LowerThanMinimumValue.json")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_GivenHigherThanMaximum()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body")
            .WithJsonBodyFromFile("Features/InputValidation/Files/HigherThanMaximumValue.json")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData("timeout=50")]
    [InlineData("timeout=50&")]
    public async Task ShouldReturnBadRequest_GivenMissingMandatoryParameterFromFormBody(string form)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body")
            .WithFormBody(form)
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData("name=test")]
    [InlineData("name=test&order=asc")]
    [InlineData("name=test&timeout=50")]
    [InlineData("name=test&order=asc&timeout=50")]
    public async Task ShouldReturnOk_WithValidFormBody(string form)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body")
            .WithFormBody(form)
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Theory]
    [InlineData("{\"name\":\"test\"}")]
    [InlineData("{\"name\":\"test\", \"order\":\"asc\"}")]
    [InlineData("{\"name\":\"test\", \"timeout\":50}")]
    [InlineData("{\"name\":\"test\", \"order\":\"asc\", \"timeout\":50}")]
    public async Task ShouldReturnOk_WithValidJsonBody(string json)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body")
            .WithJsonBody(json)
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    
    [Theory]
    [InlineData("{\"to\":[{\"type\":\"phone\", \"number\":\"lol\"}]}")]
    [InlineData("{\"to\":[{\"type\":\"sip\", \"uri\":\"lol\"}]}")]
    public async Task ShouldReturnOk_WhenOneOfArray(string json)
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Post)
            .WithUrl("/body/oneOf")
            .WithJsonBody(json)
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}