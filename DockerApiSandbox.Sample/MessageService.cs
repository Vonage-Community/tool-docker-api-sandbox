#region
using DockerApiSandbox.Sample.Components.Pages;
using DockerApiSandbox.Sample.Webhooks;
using Vonage.Messaging;
#endregion

namespace DockerApiSandbox.Sample;

public class MessageService(ISmsClient client, IWebhookService service) : IMessageService
{
    public async Task SendTextMessageAsync(Home.MessageModel message)
    {
        var sendSms = client.SendAnSmsAsync(new SendSmsRequest
        {
            To = message.To,
            From = "Sandbox project",
            Text = message.Text,
        });
        service.SubmitEvent(new MessageSent(message.To, message.Text));
        await sendSms;
        await Task.CompletedTask;
    }
}

public interface IMessageService
{
    Task SendTextMessageAsync(Home.MessageModel message);
}