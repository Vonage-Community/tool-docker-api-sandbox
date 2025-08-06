#region
using System.Net;
using System.Net.Http.Headers;
using Vonage.Common.Monads;
#endregion

namespace DockerApiSandbox.Api.Test;

internal struct HttpRequestMessageBuilder
{
    private Maybe<AuthenticationHeaderValue> authentication;
    private Maybe<StringContent> body;
    private HttpMethod method;
    private Maybe<HttpStatusCode> response;
    private string url;

    public static HttpRequestMessageBuilder Build() => new HttpRequestMessageBuilder();

    public HttpRequestMessageBuilder WithHttpMethod(HttpMethod method) => this with {method = method};

    public HttpRequestMessageBuilder WithUrl(string relativeUrl) => this with {url = relativeUrl};

    public HttpRequestMessageBuilder WithResponseHeader(HttpStatusCode expectedResponse) =>
        this with {response = expectedResponse};

    public HttpRequestMessageBuilder WithAuthorizationHeader(string scheme) =>
        this with {authentication = new AuthenticationHeaderValue(scheme, "TEST")};

    public HttpRequestMessageBuilder WithJsonBodyFromFile(string path) =>
        this with {body = new StringContent(File.ReadAllText(path), null, "application/json")};

    public HttpRequestMessageBuilder WithFormBody(string form) =>
        this with {body = new StringContent(form, null, "application/x-www-form-urlencoded")};
    
    public HttpRequestMessageBuilder WithJsonBody(string json) =>
        this with {body = new StringContent(json, null, "application/json")};

    public HttpRequestMessage Create()
    {
        var request = new HttpRequestMessage(this.method, this.url);
        this.authentication.IfSome(some => request.Headers.Authorization = some);
        this.body.IfSome(some => request.Content = some);
        this.response.IfSome(some => request.Headers.Add("Expected-Response", ((int) some).ToString()));
        return request;
    }
}