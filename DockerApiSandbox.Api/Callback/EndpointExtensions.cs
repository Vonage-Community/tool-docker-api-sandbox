using NSwag;

namespace DockerApiSandbox.Api.Callback;

public static class EndpointExtensions
{
    public static bool HasCallbacks(this Endpoint endpoint) => endpoint.Operation.Callbacks != null;

    public static IEnumerable<KeyValuePair<string, OpenApiOperation>> GetCallbacks(this Endpoint endpoint) => 
        from callback 
            in endpoint.Operation.Callbacks 
        from callbackUrl 
            in callback.Value 
        from operation 
            in callbackUrl.Value 
        select operation;
}