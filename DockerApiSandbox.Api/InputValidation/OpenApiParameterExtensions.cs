#region
using Newtonsoft.Json;
using NSwag;
#endregion

namespace DockerApiSandbox.Api.InputValidation;

public static class OpenApiParameterExtensions
{
    public static bool IsQueryParameterValid(this OpenApiParameter parameter, IQueryCollection query)
    {
        if (!query.TryGetValue(parameter.Name, out var value))
        {
            return !parameter.IsRequired;
        }

        var queryParameter = value.ToString().ConvertToType(parameter.Schema.Type);
        if (queryParameter is null)
        {
            return false;
        }

        return !parameter.Schema.Validate(JsonConvert.SerializeObject(queryParameter)).Any();
    }
}