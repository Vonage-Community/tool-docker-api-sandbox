using System.Net;
using Xunit.Abstractions;

namespace DockerApiSandbox.Api.Test.Products.Messages;

public class MessagesTest(ITestOutputHelper helper)
{
    private readonly TestApplicationFactory<Program> application = TestApplicationFactory<Program>.Builder(helper)
        .Build();
    
    [Theory]
    [InlineData("Bearer", "Products/Messages/Files/SendRcsText.json")]
    [InlineData("Basic", "Products/Messages/Files/SendRcsText.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendRcsCustom.json")]
    [InlineData("Basic", "Products/Messages/Files/SendRcsCustom.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendRcsFile.json")]
    [InlineData("Basic", "Products/Messages/Files/SendRcsFile.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendRcsImage.json")]
    [InlineData("Basic", "Products/Messages/Files/SendRcsImage.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendRcsVideo.json")]
    [InlineData("Basic", "Products/Messages/Files/SendRcsVideo.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendSms.json")]
    [InlineData("Basic", "Products/Messages/Files/SendSms.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMmsAudio.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMmsAudio.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMmsFile.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMmsFile.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMmsImage.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMmsImage.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMmsText.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMmsText.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMmsVcard.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMmsVcard.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMmsVideo.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMmsVideo.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMessengerVideo.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMessengerVideo.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMessengerFile.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMessengerFile.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMessengerImage.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMessengerImage.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMessengerText.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMessengerText.json")]
    [InlineData("Bearer", "Products/Messages/Files/SendMessengerAudio.json")]
    [InlineData("Basic", "Products/Messages/Files/SendMessengerAudio.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppAudio.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppCustom.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppFile.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppImage.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppReaction.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppSticker.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppTemplate.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppText.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWhatsAppVideo.json")]
    [InlineData("Basic", "Products/Messages/Files/SendViberFile.json")]
    [InlineData("Basic", "Products/Messages/Files/SendViberImage.json")]
    [InlineData("Basic", "Products/Messages/Files/SendViberText.json")]
    [InlineData("Basic", "Products/Messages/Files/SendViberVideo.json")]
    [InlineData("Basic", "Products/Messages/Files/SendWithFailover.json")]
    public async Task SendMessage_ShouldReturnAccepted(string auth, string filepath)
    {
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/v1/messages")
            .WithAuthorizationHeader(auth)
            .WithJsonBodyFromFile(Path.GetFullPath(filepath))
            .Create();
        var response = await this.application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}