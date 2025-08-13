#region
using Newtonsoft.Json;
using NSwag;
#endregion

namespace DockerApiSandbox.Api.InputValidation;

public static class OpenApiParameterExtensions
{
    public static IEnumerable<string> GetQueryParameterErrors(this OpenApiParameter parameter, IQueryCollection query)
    {
        if (!query.TryGetValue(parameter.Name, out var value))
        {
            return parameter.IsRequired
                ? [$"Missing required query parameter: {parameter.Name}"]
                : [];
        }

        var queryParameter = value.ToString().ConvertToType(parameter.Schema.Type);
        if (queryParameter is null)
        {
            return [$"Could not convert '{value}' to '{parameter.Schema.Type}'"];
        }

        return parameter.Schema.Validate(JsonConvert.SerializeObject(queryParameter)).Select(error => error.Format());
    }
}