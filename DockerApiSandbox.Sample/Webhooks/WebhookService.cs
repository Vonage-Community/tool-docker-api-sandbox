namespace DockerApiSandbox.Sample.Webhooks;

public class WebhookService : IWebhookService
{
    public event Action<IEvent> OnEventReceived;
    public void SubmitEvent(IEvent @event) => this.OnEventReceived?.Invoke(@event);
}

public interface IWebhookService
{
    event Action<IEvent> OnEventReceived;
    void SubmitEvent(IEvent @event);
}