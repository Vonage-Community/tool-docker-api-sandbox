#region
using Vonage.Common.Failures;
using Vonage.Common.Monads;
#endregion

namespace DockerApiSandbox.Api.AuthenticationVerification;

public class AuthenticationVerificationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<AuthenticationVerificationMiddleware> logger)
    {
        var securitySchemes = Endpoint.FromHttpContext(context).RetrieveSecuritySchemes().ToList();
        if (securitySchemes.Count == 0)
        {
            LogSuccessfulAuthentication(logger);
            await this.ProceedToNextMiddleware(context);
            return;
        }

        await GetAuthorizationHeader(context.Request.Headers)
            .Map(SplitHeader)
            .Bind(ExtractScheme)
            .Bind(scheme => IsSchemeAllowed(securitySchemes, scheme))
            .Do(_ => LogSuccessfulAuthentication(logger), failure => LogFailedAuthentication(logger, failure))
            .Match(async _ => await this.ProceedToNextMiddleware(context),
                failure => SetResponseAsFailure(context, failure));
    }

    private static Result<Unit> IsSchemeAllowed(List<string> schemes, string scheme) =>
        schemes.Contains(scheme.ToLowerInvariant())
            ? Unit.Default
            : Result<Unit>.FromFailure(ResultFailure.FromErrorMessage($"Scheme '{scheme}' is not allowed"));

    private static void
        LogFailedAuthentication(ILogger<AuthenticationVerificationMiddleware> logger, IResultFailure failure) =>
        logger.LogError("Authentication failed: {FailureMessage}", failure.GetFailureMessage());

    private static void LogSuccessfulAuthentication(ILogger<AuthenticationVerificationMiddleware> logger) =>
        logger.LogInformation("Authenticated");

    private async Task<Unit> ProceedToNextMiddleware(HttpContext context)
    {
        await next(context);
        return Unit.Default;
    }

    private static async Task<Unit> SetResponseAsFailure(HttpContext context, IResultFailure failure)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync(failure.GetFailureMessage());
        return Unit.Default;
    }

    private static Result<string> GetAuthorizationHeader(IHeaderDictionary headers) =>
        headers.Authorization.Count == 0
            ? Result<string>.FromFailure(ResultFailure.FromErrorMessage("No Authorization header found"))
            : headers.Authorization.ToString();

    private static string[] SplitHeader(string header) => header.Split(' ', 2);

    private static Result<string> ExtractScheme(string[] header) =>
        header.Length != 2
            ? Result<string>.FromFailure(ResultFailure.FromErrorMessage("Invalid scheme format"))
            : header[0];
}