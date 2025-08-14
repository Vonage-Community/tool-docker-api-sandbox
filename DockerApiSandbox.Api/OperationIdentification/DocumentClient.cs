#region
using NSwag;
using Polly;
using Vonage.Common.Monads;
#endregion

namespace DockerApiSandbox.Api.OperationIdentification;

public class DocumentClient(ILogger<DocumentClient> logger)
{
    private const int RetryCount = 3;

    private readonly AsyncPolicy<OpenApiDocument> retryPolicy = Policy<OpenApiDocument>
        .Handle<Exception>()
        .WaitAndRetryAsync(
            RetryCount,
            EvaluateTimeBeforeRetry,
            (_, timespan, retryAttempt, _) => logger.LogWarning("Retry {RetryAttempt} for document download. Waiting {TimeSpan} before next attempt.", retryAttempt, timespan));

    public async Task<Maybe<OpenApiDocument>> DownloadDocument(ApiSpecification spec) => await ParseUri(spec.Url)
        .BindAsync(uri => uri.Scheme == "file" ? this.DownloadFromFile(uri) : this.DownloadFromUri(uri));

    private async Task<Maybe<OpenApiDocument>> DownloadFromFile(Uri uri)
    {
        logger.LogInformation($"Downloading document from path {uri}...");
        try
        {
            return await OpenApiDocument.FromFileAsync(uri.AbsolutePath);
        }
        catch (Exception exception)
        {
            logger.LogInformation($"Failed downloading document from path {uri} because {exception.Message}");
            return Maybe<OpenApiDocument>.None;
        }
    }

    private async Task<Maybe<OpenApiDocument>> DownloadFromUri(Uri uri)
    {
        logger.LogInformation($"Downloading document from URI {uri}...");
        try
        {
            return await this.retryPolicy.ExecuteAsync(async () => await OpenApiDocument.FromUrlAsync(uri.AbsoluteUri));
        }
        catch (Exception exception)
        {
            logger.LogInformation($"Failed downloading document from URI {uri} because {exception.Message}");
            return Maybe<OpenApiDocument>.None;
        }
    }

    private static TimeSpan EvaluateTimeBeforeRetry(int retryAttempt) =>
        TimeSpan.FromSeconds(retryAttempt) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000));

    private static Maybe<Uri> ParseUri(string path) => Uri.TryCreate(path, UriKind.Absolute, out var uri) ? uri : Maybe<Uri>.None;
}