#region
using Microsoft.AspNetCore.Mvc;
#endregion

namespace DockerApiSandbox.Sample.Webhooks;

[ApiController]
[Route("api/[controller]")]
public class WebhookController(IWebhookService webhookService) : ControllerBase
{
    [HttpPost]
    public IActionResult ReceiveWebhook([FromBody] WebhookReceived callback)
    {
        webhookService.SubmitEvent(callback);
        return this.Ok();
    }
}