#region
using System.Text;
using DockerApiSandbox.Api.OperationIdentification;
using NSwag;
#endregion

namespace DockerApiSandbox.Api.Callback;

public class CallbackMiddleware(RequestDelegate next)
{
    private const string ContentTypeJson = "application/json";

    private static readonly Dictionary<OperationCallback, string> CallbackMapping =
        new Dictionary<OperationCallback, string>
        {
            {new OperationCallback(SupportedApi.Sms, "delivery-receipt"), "SMS_DLR"},
        };

    public async Task InvokeAsync(HttpContext context, ILogger<CallbackMiddleware> logger,
        IEnvironmentAdapter environment)
    {
        var endpoint = Endpoint.FromHttpContext(context);
        if (!endpoint.HasCallbacks())
        {
            return;
        }

        var tasks = endpoint.GetCallbacks()
            .Select(operation => this.TriggerCallback(logger, environment, operation, endpoint));
        await Task.WhenAll(tasks);
    }

    private async Task TriggerCallback(ILogger<CallbackMiddleware> logger, IEnvironmentAdapter environment,
        KeyValuePair<string, OpenApiOperation> operation,
        Endpoint endpoint)
    {
        var callback = new OperationCallback(endpoint.Api, operation.Value.OperationId);
        if (CallbackMapping.TryGetValue(callback, out var value) && environment.HasVariable(value))
        {
            var httpRequestMessage =
                new HttpRequestMessage(new HttpMethod(operation.Key.ToUpperInvariant()),
                    new Uri(environment.GetVariable(value)))
                {
                    Content = new StringContent(
                        operation.Value.RequestBody.Content[ContentTypeJson].Schema.ToSampleJson().ToString(),
                        Encoding.UTF8,
                        ContentTypeJson),
                };
            await this.TrySendRequest(logger, httpRequestMessage);
        }
    }

    private async Task TrySendRequest(ILogger<CallbackMiddleware> logger, HttpRequestMessage some)
    {
        try
        {
            logger.LogInformation("Sending callback to {SomeRequestUri}", some.RequestUri);
            var response = await new HttpClient().SendAsync(some);
            logger.LogInformation("Received {ResponseStatusCode} from {SomeRequestUri}", response.StatusCode,
                some.RequestUri);
        }
        catch (Exception exception)
        {
            logger.LogError(exception.Message);
        }
    }

    private record OperationCallback(SupportedApi Api, string OperationId);
}