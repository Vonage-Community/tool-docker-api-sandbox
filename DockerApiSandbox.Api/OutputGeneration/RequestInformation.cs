using NSwag;
using Vonage.Common.Monads;

namespace DockerApiSandbox.Api.OutputGeneration;

public record RequestInformation(OpenApiOperation Operation, Maybe<string> ExpectedResponse)
{
    public Maybe<ResponseSchema> GetResponseBodySchema() =>
        this.ExpectedResponse.Match(
                some => this.Operation.Responses.Where(response => response.Key.Equals(some)),
                () => this.Operation.Responses.Where(response => response.Key.StartsWith('2'))
            )
            .OrderBy(response => response.Value.Content.ContainsKey("application/json") ? 0 : 1)
            .Select(response => new ResponseSchema(response.Key, response.Value))
            .FirstOrDefault() ?? Maybe<ResponseSchema>.None;
}