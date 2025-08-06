namespace DockerApiSandbox.Api.InputValidation;

public class InputValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<InputValidationMiddleware> logger)
    {
        var endpoint = Endpoint.FromHttpContext(context);
        if (await context.Request.IsRequestBodyValid(endpoint.Operation) &&
            context.Request.IsRequestQueryValid(endpoint.Operation))
        {
            logger.LogInformation("Input validated");
            await next(context);
        }
        else
        {
            logger.LogError("Input validation failed");
            await WriteBadRequestResponse(context);
        }
    }

    private static async Task WriteBadRequestResponse(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Input validation failed");
    }
}