namespace DockerApiSandbox.Api.OutputGeneration;

public static class HttpContextExtensions
{
    public static RequestInformation ExtractRequestInformation(this HttpContext context) =>
        new RequestInformation(Endpoint.FromHttpContext(context).Operation, context.Request.ExtractExpectedResponse());
}
