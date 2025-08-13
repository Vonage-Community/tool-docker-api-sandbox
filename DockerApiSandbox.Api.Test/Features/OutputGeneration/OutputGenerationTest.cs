#region
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Features.OutputGeneration;

public class OutputGenerationTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper)
        .OverrideApplicationSpec(Path.GetFullPath("Features/OutputGeneration/Files/spec.json"))
        .WithEnvironmentVariable("CLEAR_SPECS", "true")
        .Build();
    
    [Fact]
    public async Task ShouldReturnOk_WithRequiredFormParameter()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Get)
            .WithUrl("/generation")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await ExtractContent(response);
        content.ContainsKey("Status").Should().BeTrue();
        content.ContainsKey("Count").Should().BeTrue();
        content["Status"].GetString().Should().Be("Success");
        content["Count"].GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task ShouldReturnOtherStatusCode_GivenOverridenExpectedResponse()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Get)
            .WithUrl("/generation")
            .WithResponseHeader(HttpStatusCode.BadRequest)
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await ExtractContent(response);
        content.ContainsKey("Message").Should().BeTrue();
        content["Message"].GetString().Should().Be("Bad Request");
    }

    [Fact]
    public async Task ShouldReturnNoContent_GivenExpectedResponseDoesNotMatch()
    {
        var response = await this.application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build().WithHttpMethod(HttpMethod.Get)
            .WithUrl("/generation")
            .WithResponseHeader(HttpStatusCode.Continue)
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<Dictionary<string, JsonElement>> ExtractContent(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
    }
}