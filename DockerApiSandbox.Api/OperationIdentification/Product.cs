using Microsoft.AspNetCore.Http.Extensions;
using NSwag;
using Vonage.Common.Monads;

namespace DockerApiSandbox.Api.OperationIdentification;

public record Product(SupportedApi Api, OpenApiDocument Specification)
{
    public IEnumerable<Maybe<Endpoint>> FindOperation(HttpRequest request)
    {
        var requestUri = new Uri(request.GetDisplayUrl());
        var method = request.Method.ToLowerInvariant();
        var serverUrl = new Uri(this.Specification.Servers.First().Url);
        var paths = this.Specification.Paths.Select(path =>
            new KeyValuePair<string, OpenApiPathItem>(CombinePaths(serverUrl.AbsolutePath, path.Key), path.Value));
        foreach (var item in paths
                     .Where(item => PathsMatch(item.Key, requestUri.AbsolutePath))
                     .OrderBy(OrderBySpecificity))
        {
            var operation = item.Value.TryGetValue(method, out var value) ? value : Maybe<OpenApiOperation>.None;
            var endpoint = operation.Map(some =>
                new Endpoint(this.Api, this.Specification.Components.SecuritySchemes, some));
            yield return endpoint;
        }
    }

    private static int OrderBySpecificity(KeyValuePair<string, OpenApiPathItem> endpoint) => endpoint.Key.Contains('{') ? 1 : 0;

    private static string CombinePaths(string basePath, string relativePath) =>
        string.IsNullOrEmpty(basePath) || basePath == "/"
            ? relativePath
            : basePath.TrimEnd('/') + "/" + relativePath.TrimStart('/');

    private static bool PathsMatch(string templatePath, string requestPath)
    {
        var templateSegments = templatePath.Trim('/').Split('/');
        var requestSegments = requestPath.Trim('/').Split('/');
        return templateSegments.Length == requestSegments.Length &&
               templateSegments.Zip(requestSegments, SegmentMatches).All(match => match);
    }

    private static bool SegmentMatches(string templateSegment, string requestSegment) =>
        (templateSegment.StartsWith("{") && templateSegment.EndsWith("}")) ||
        string.Equals(templateSegment, requestSegment, StringComparison.OrdinalIgnoreCase);
}