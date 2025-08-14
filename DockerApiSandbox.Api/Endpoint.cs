using DockerApiSandbox.Api.OperationIdentification;
using NSwag;

namespace DockerApiSandbox.Api;

public record Endpoint(
    SupportedApi Api,
    IDictionary<string, OpenApiSecurityScheme> SecuritySchemes,
    OpenApiOperation Operation)
{
    public IEnumerable<string> RetrieveSecuritySchemes() =>
        this.Operation.Security?
            .SelectMany(requirement => requirement.Keys)
            .Where(this.SecuritySchemes.ContainsKey)
            .Select(schemeKey => this.SecuritySchemes[schemeKey].Scheme.ToLowerInvariant())
        ?? [];
    
    public static Endpoint FromHttpContext(HttpContext context) =>
        (Endpoint) context.Items["endpoint"]!;
};