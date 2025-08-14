#region
using System.Text.Json;
using Newtonsoft.Json;
using Vonage.Common.Monads;
using JsonSerializer = System.Text.Json.JsonSerializer;
#endregion

namespace DockerApiSandbox.Api.OutputGeneration;

public class OutputGenerationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<OutputGenerationMiddleware> logger)
    {
        _ = await context.ExtractRequestInformation()
            .GetResponseBodySchema()
            .Map(schema => schema.GenerateData())
            .Do(
                some => LogGeneratedData(logger, some),
                () => LogFailedGeneration(logger)
            )
            .Match(
                some => WriteResponse(context, some),
                () => WriteNoContentResponse(context)
            );
        await next(context);
    }

    private static void LogFailedGeneration(ILogger<OutputGenerationMiddleware> logger) =>
        logger.LogError("Returning no content");

    private static void LogGeneratedData(ILogger<OutputGenerationMiddleware> logger, ResponseData some) =>
        some.Response.IfSome(output => logger.LogInformation("Data generated:\n{Serialize}",
            JsonSerializer.Serialize(output, new JsonSerializerOptions {WriteIndented = true})));

    private static Task<Unit> WriteNoContentResponse(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status204NoContent;
        return Task.FromResult(Unit.Default);
    }

    private static async Task<Unit> WriteResponse(HttpContext context, ResponseData content)
    {
        context.Response.StatusCode = int.Parse(content.StatusCode);
        await content.Response.IfSomeAsync(async some =>
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(some));
        });
        return Unit.Default;
    }
}