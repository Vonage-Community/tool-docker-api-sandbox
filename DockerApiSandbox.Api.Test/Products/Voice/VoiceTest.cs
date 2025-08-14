#region
using Xunit.Abstractions;
#endregion

namespace DockerApiSandbox.Api.Test.Products.Voice;

public class VoiceTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application =
        TestApplicationFactory<Program>.Builder(helper)
            .OverrideVoiceSpec(Path.GetFullPath("Products/Voice/Files/voice_local_fixed.json"))
            .Build();

    [Theory]
    [InlineData("CreateCall_AnswerUrl_Phone")]
    [InlineData("CreateCall_NCCO_Phone")]
    [InlineData("CreateCall_NCCO_Sip")]
    [InlineData("CreateCall_NCCO_Vbc")]
    [InlineData("CreateCall_NCCO_Websocket")]
    public async Task CreateCall_ShouldReturnOk(string filename)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v1/calls")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath($"Products/Voice/Files/{filename}.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Theory]
    [InlineData("Modify_Earmuff")]
    [InlineData("Modify_Unearmuff")]
    [InlineData("Modify_Mute")]
    [InlineData("Modify_Unmute")]
    [InlineData("Modify_Hangup")]
    [InlineData("Modify_TransferAnswerUrl")]
    [InlineData("Modify_TransferNcco")]
    public async Task ModifyCall_ShouldReturnOk(string filename)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Put)
            .WithUrl("/v1/calls/CALL-123")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath($"Products/Voice/Files/{filename}.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetCall_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/calls/CALL-123")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCalls_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Get)
            .WithUrl("/v1/calls")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task StreamAudio_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Put)
            .WithUrl("/v1/calls/CALL-123/stream")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Voice/Files/StreamAudio.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task StopStreamAudio_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v1/calls/CALL-123/stream")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task PlayTextToSpeech_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Put)
            .WithUrl("/v1/calls/CALL-123/talk")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Voice/Files/PlayTextToSpeech.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task StopTextToSpeech_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v1/calls/CALL-123/talk")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task PlayDtmf_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Put)
            .WithUrl("/v1/calls/CALL-123/dtmf")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Voice/Files/PlayDtmf.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task SubscribeToRealTimeEvents_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Put)
            .WithUrl("/v1/calls/CALL-123/input/dtmf")
            .WithAuthorizationHeader("Bearer")
            .WithJsonBodyFromFile(Path.GetFullPath("Products/Voice/Files/SubscribeToRealTimeEvents.json"))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task UnsubscribeToRealTimeEvents_ShouldReturnOk()
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Delete)
            .WithUrl("/v1/calls/CALL-123/input/dtmf")
            .WithAuthorizationHeader("Bearer")
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}