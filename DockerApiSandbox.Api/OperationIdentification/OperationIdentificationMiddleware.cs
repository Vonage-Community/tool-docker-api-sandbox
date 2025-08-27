#region
using Vonage.Common.Monads;
#endregion

namespace DockerApiSandbox.Api.OperationIdentification;

internal class OperationIdentificationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IDocumentStore store,
        ILogger<OperationIdentificationMiddleware> logger)
    {
        var products = await Task.WhenAll(store.LoadDocuments());
        var endpoints = products
            .WhereSome()
            .SelectMany(product => product.FindOperation(context.Request))
            .WhereSome()
            .ToArray();
        _ = endpoints.Length != 0
            ? await this.ProceedToNextMiddleware(context, logger, endpoints.First())
            : await WriteErrorResponse(context, logger);
    }

    private async Task<Unit> ProceedToNextMiddleware(HttpContext context,
        ILogger<OperationIdentificationMiddleware> logger, Endpoint some)
    {
        context.Items["endpoint"] = some;
        logger.LogInformation("Operation identified: {Api} | {Id}", some.Api, some.Operation.OperationId);
        await next(context);
        return Unit.Default;
    }

    private static async Task<Unit> WriteErrorResponse(HttpContext context,
        ILogger<OperationIdentificationMiddleware> logger)
    {
        logger.LogInformation("No operation found for the incoming request");
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync("Operation not found.");
        return Unit.Default;
    }
}