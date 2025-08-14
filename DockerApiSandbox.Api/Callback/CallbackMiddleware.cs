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
        _ = next;
    }

    private async Task TriggerCallback(ILogger<CallbackMiddleware> logger, IEnvironmentAdapter environment,
        KeyValuePair<string, OpenApiOperation> operation,
        Endpoint endpoint)
    {
        var callback = new OperationCallback(endpoint.Api, operation.Value.OperationId);
        if (CallbackMapping.TryGetValue(callback, out var value) && environment.HasVariable(value))
        {
            await environment.GetVariable(value)
                .Map(variable => new Uri(variable))
                .Map(uri => new HttpRequestMessage(new HttpMethod(operation.Key.ToUpperInvariant()), uri)
                {
                    Content = new StringContent(
                        operation.Value.RequestBody.Content[ContentTypeJson].Schema.ToSampleJson().ToString(),
                        Encoding.UTF8,
                        ContentTypeJson),
                })
                .IfSomeAsync( request => this.TrySendRequest(logger, request));
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