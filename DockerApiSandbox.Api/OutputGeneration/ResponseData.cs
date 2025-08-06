using Vonage.Common.Monads;

namespace DockerApiSandbox.Api.OutputGeneration;

public record ResponseData(string StatusCode, Maybe<object> Response);