using NJsonSchema;

namespace DockerApiSandbox.Api.InputValidation;

public static class StringExtensions
{
    public static object ConvertToType(this string value, JsonObjectType expectedType) =>
        expectedType switch
        {
            JsonObjectType.Integer => int.TryParse(value, out var intValue) ? intValue : null,
            JsonObjectType.Number => double.TryParse(value, out var doubleValue) ? doubleValue : null,
            JsonObjectType.Boolean => bool.TryParse(value, out var boolValue) ? boolValue : null,
            _ => value,
        };
}