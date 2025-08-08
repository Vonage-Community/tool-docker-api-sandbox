namespace DockerApiSandbox.Sample.Webhooks;

public record MessageSent(string To, string Text) : IEvent
{
    public string Message => $"Message '{this.Text}' sent to '{this.To}'.";
}

public record WebhookReceived(string To, string Status) : IEvent
{
    public string Message => $"Message delivered: Webhook received for {this.To} with status {this.Status}";
}

public interface IEvent
{
    string Message { get; }
}