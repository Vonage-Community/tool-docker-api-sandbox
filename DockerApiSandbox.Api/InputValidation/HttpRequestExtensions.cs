#region
using System.Web;
using Newtonsoft.Json;
using NSwag;
#endregion

namespace DockerApiSandbox.Api.InputValidation;

public static class HttpRequestExtensions
{
    private const string ContentTypeForm = "application/x-www-form-urlencoded";
    private const string ContentTypeJson = "application/json";

    public static async Task<IEnumerable<string>> GetRequestBodyErrors(this HttpRequest request, OpenApiOperation operation)
    {
        if (operation.RequestBody == null)
        {
            return [];
        }

        var bodyContent = await request.GetRequestBody();
        if (string.IsNullOrEmpty(bodyContent))
        {
            return ["Missing body"];
        }

        var contentType = request.GetContentType();
        var contentSchema = operation.ActualRequestBody.Content
            .FirstOrDefault(content => content.Key.Equals(contentType, StringComparison.OrdinalIgnoreCase)).Value;
        var jsonBody = contentType switch
        {
            ContentTypeForm => SerializeFormUrlEncodedIntoJson(operation, bodyContent),
            ContentTypeJson => bodyContent,
            _ => string.Empty,
        };
        return contentSchema.Schema.Validate(jsonBody).Select(error => error.Format());
    }

    public static IEnumerable<string> GetRequestQueryErrors(this HttpRequest request, OpenApiOperation operation) =>
        operation.Parameters.Where(parameter => parameter.Kind == OpenApiParameterKind.Query)
            .SelectMany(parameter => parameter.GetQueryParameterErrors(request.Query));

    private static string GetContentType(this HttpRequest request) =>
        request.ContentType?.Split(';').FirstOrDefault()?.Trim().ToLower();

    private static async Task<string> GetRequestBody(this HttpRequest request)
    {
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private static string SerializeFormUrlEncodedIntoJson(OpenApiOperation operation, string bodyContent)
    {
        var formData = HttpUtility.ParseQueryString(bodyContent);
        var schema = operation.Parameters.First(parameter => parameter.Kind == OpenApiParameterKind.Body).ActualSchema;
        var valueDictionary = formData.AllKeys.Where(key => !string.IsNullOrEmpty(key))
            .ToDictionary(key => key, value => formData[value])
            .ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ConvertToType(schema.Properties.First(jsonProperty => jsonProperty.Key == pair.Key)
                    .Value.Type));
        return JsonConvert.SerializeObject(valueDictionary);
    }
}