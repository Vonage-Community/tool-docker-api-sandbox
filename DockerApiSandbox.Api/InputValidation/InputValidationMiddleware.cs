namespace DockerApiSandbox.Api.InputValidation;

public class InputValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<InputValidationMiddleware> logger)
    {
        var errors = await GetErrors(context);
        if (errors.Length == 0)
        {
            logger.LogInformation("Input validated");
            await next(context);
        }
        else
        {
            logger.LogError($"Input validation failed: {string.Join('|', errors)}");
            await WriteBadRequestResponse(context);
        }
    }

    private static async Task<string[]> GetErrors(HttpContext context)
    {
        var endpoint = Endpoint.FromHttpContext(context);
        var queryErrors = context.Request.GetRequestQueryErrors(endpoint.Operation);
        var bodyErrors = (await context.Request.GetRequestBodyErrors(endpoint.Operation));
        return queryErrors.Concat(bodyErrors).ToArray();
    }

    private static async Task WriteBadRequestResponse(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Input validation failed");
    }
}