using Vonage.Common.Monads;

namespace DockerApiSandbox.Api.OutputGeneration;

public static class HttpRequestExtensions
{
    public static Maybe<string> ExtractExpectedResponse(this HttpRequest request) =>
        request.Headers.TryGetValue("Expected-Response", out var value)
            ? value.FirstOrDefault() ?? Maybe<string>.None
            : Maybe<string>.None;
}