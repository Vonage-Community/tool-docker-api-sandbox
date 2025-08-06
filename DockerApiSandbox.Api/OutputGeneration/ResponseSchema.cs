using NJsonSchema;
using NSwag;
using Vonage.Common.Monads;

namespace DockerApiSandbox.Api.OutputGeneration;

public record ResponseSchema(string StatusCode, OpenApiResponse Response)
{
    private Maybe<JsonSchema> GetSchema() =>
        this.Response.Content.ContainsKey("application/json")
            ? this.Response.Content["application/json"].Schema
            : Maybe<JsonSchema>.None;

    public ResponseData GenerateData() =>
        this.GetSchema()
            .Map(schema => schema.OneOf.Count > 0 ? schema.OneOf.First() : schema)
            .Map(schema => new ResponseData(this.StatusCode, DataGenerator.GenerateData(schema.ActualSchema)))
            .IfNone(new ResponseData(this.StatusCode, Maybe<object>.None));
}