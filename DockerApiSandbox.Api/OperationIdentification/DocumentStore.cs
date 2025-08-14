#region
using System.ComponentModel;
using System.Runtime.Serialization;
using EnumsNET;
using Microsoft.Extensions.Caching.Memory;
using NSwag;
using Vonage.Common.Monads;
#endregion

namespace DockerApiSandbox.Api.OperationIdentification;

internal class DocumentStore(
    IMemoryCache cache,
    ILogger<DocumentStore> logger,
    IConfiguration configuration,
    DocumentClient documentClient,
    IEnvironmentAdapter environment)
{
    public IEnumerable<Task<Maybe<Product>>> LoadDocuments()
    {
        var uris = environment.GetVariable("CLEAR_SPECS").IfNone(string.Empty) switch
        {
            "true" => GetUrisFromEnvironment(environment),
            _ => GetUrisFromConfiguration(configuration).Select(uri => uri.Overwrite(environment)),
        };
        return uris.Select(this.FetchDocument);
    }

    private void AddDocumentToCache(SupportedApi api, OpenApiDocument document)
    {
        cache.Set(api, document, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
        });
        logger.LogInformation($"Document for {api} added to cache");
    }

    private async Task<Maybe<Product>> FetchDocument(ApiSpecification spec)
    {
        if (!cache.TryGetValue(spec.SupportedApi, out Maybe<OpenApiDocument> document))
        {
            document = await documentClient.DownloadDocument(spec);
            document.IfSome(some => this.AddDocumentToCache(spec.SupportedApi, some));
        }

        return document.Map(some => new Product(spec.SupportedApi, some));
    }

    private static IEnumerable<ApiSpecification> GetUrisFromConfiguration(IConfiguration configuration) =>
        configuration.GetSection("specs").Get<List<ApiSpecification>>() ?? [];

    private static IEnumerable<ApiSpecification> GetUrisFromEnvironment(IEnvironmentAdapter environment) =>
        Enum.GetValues<SupportedApi>()
            .Select(api => new ApiSpecification(api,
                environment.GetVariable(api.AsString(EnumFormat.Description) ?? string.Empty).IfNone(string.Empty)))
            .Where(spec => !string.IsNullOrEmpty(spec.Url));
}

public enum SupportedApi
{
    [EnumMember(Value = "Sms")] [Description("SPEC_SMS")]
    Sms,

    [EnumMember(Value = "Application")] [Description("SPEC_APPLICATION")]
    Application,

    [EnumMember(Value = "Voice")] [Description("SPEC_VOICE")]
    Voice,

    [EnumMember(Value = "VerifyV2")] [Description("SPEC_VERIFY")]
    VerifyV2,

    [EnumMember(Value = "Messages")] [Description("SPEC_MESSAGES")]
    Messages,
}