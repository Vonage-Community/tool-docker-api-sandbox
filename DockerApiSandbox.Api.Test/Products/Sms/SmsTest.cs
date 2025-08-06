#region
using System.Net;
using WireMock.Server;
#endregion

namespace DockerApiSandbox.Api.Test.Products.Sms;

public class SmsTest
{
    [Fact]
    public async Task SendSms_ShouldReturnOk_WithAllParameters()
    {
        var application = TestApplicationFactory<Program>.Builder().Build();
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/sms/json")
            .WithFormBody("api_key=12345678&api_secret=12345678&sig=1234567890123456&from=AcmeInc&to=447700900001&text=Hello+World!&ttl=900000&status-report-req=false&callback=https://example.com/sms-dlr&message-class=0&type=text&body=0011223344556677&udh=06050415811581&protocol-id=127&client-ref=my-personal-reference&account-ref=customer1234&entity-id=1101456324675322134&content-id=1107457532145798767")
            .Create();
        var response = await application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task SendSms_ShouldReturnOk_WithMandatoryParameters()
    {
        var application = TestApplicationFactory<Program>.Builder().Build();
        var request = HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/sms/json")
            .WithFormBody("api_key=12345678&from=447700900000&to=447700900001")
            .Create();
        var response = await application.CreateClient().SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task SendSms_ShouldSendCallback()
    {
        var callbackServer = WireMockServer.Start();
        var callbackUrl = callbackServer.Url + "/sms/delivery-receipt";
        var application = TestApplicationFactory<Program>.Builder().WithEnvironmentVariable("SMS_DLR", callbackUrl).Build();
        var response = await application.CreateClient().SendAsync(HttpRequestMessageBuilder.Build()
            .WithHttpMethod(HttpMethod.Post)
            .WithUrl("/sms/json")
            .WithFormBody("api_key=12345678&from=447700900000&to=447700900001")
            .Create());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        callbackServer.LogEntries.Any(entry => entry.RequestMessage.Url == callbackUrl).Should().BeTrue();
    }
}