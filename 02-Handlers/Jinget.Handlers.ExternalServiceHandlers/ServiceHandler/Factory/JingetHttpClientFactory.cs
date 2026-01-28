using Jinget.Core.ExtensionMethods;

using System.Threading;

namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory;

/// <summary>
/// Factory for creating and configuring HttpClient instances.
/// </summary>
public class JingetHttpClientFactory(IHttpClientFactory httpClientFactory)
{
    /// <summary>
    /// Gets or sets the named HttpClient client.
    /// </summary>
    internal string? clientName;
    /// <summary>
    /// Gets or sets the base URI for HttpClient.
    /// </summary>
    internal string? baseUri;
    /// <summary>
    /// Gets or sets the timeout for HttpClient requests.
    /// </summary>
    internal TimeSpan? timeout;

    /// <summary>
    /// Creates a configured HttpClient instance.
    /// </summary>
    /// <returns>The created HttpClient.</returns>
    private HttpClient CreateHttpClient()
    {
        var httpClient = httpClientFactory.CreateClient(clientName);
        if (!string.IsNullOrWhiteSpace(baseUri)) httpClient.BaseAddress = new Uri(baseUri);
        if (timeout.HasValue) httpClient.Timeout = timeout.Value;
        httpClient.DefaultRequestHeaders.ConnectionClose = false;
        return httpClient;
    }

    /// <summary>
    /// Constructs a request URI.
    /// </summary>
    /// <param name="relativeUrl">The relative URL.</param>
    /// <returns>The constructed URI.</returns>
    private Uri GetRequestUri(string relativeUrl) => string.IsNullOrWhiteSpace(baseUri) ? new($"{relativeUrl}".TrimEnd('/')) : new($"{baseUri.TrimEnd('/')}/{relativeUrl}".TrimEnd('/'));

    /// <summary>
    /// Sets request headers, including authorization.
    /// </summary>
    /// <param name="requestHeaders">The request headers.</param>
    /// <param name="httpClient">The HttpClient instance.</param>
    /// <returns>The set request headers.</returns>
    private static Dictionary<string, string> SetRequestHeaders(Dictionary<string, string>? requestHeaders, HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Clear();
        requestHeaders ??= new Dictionary<string, string> { { "Content-type", "application/json; charset=utf-8" } };
        foreach (var header in requestHeaders)
        {
            if (Not(header.Key.Equals("Authorization", StringComparison.InvariantCultureIgnoreCase))) httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            else if (header.Value.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase)) httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", header.Value[7..]);
            else if (header.Value.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase)) httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", header.Value[6..]);
            else if (header.Value.StartsWith("Digest ", StringComparison.InvariantCultureIgnoreCase)) httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Digest", header.Value[7..]);
        }
        return requestHeaders;
    }

    /// <summary>
    /// Uploads files using multipart form data.
    /// </summary>
    /// <param name="requestUrl">The request URL.</param>
    /// <param name="fileInfos">The list of file information.</param>
    /// <param name="requestHeaders">The request headers.</param>
    /// <returns>The HTTP response message.</returns>
    public async Task<HttpResponseMessage> UploadFilesAsync(
        string requestUrl,
        List<FileInfo>? fileInfos = null,
        Dictionary<string, string>? requestHeaders = null,
        CancellationToken cancellationToken = default)
    {
        using var multipartFormContent = new MultipartFormDataContent();
        if (fileInfos is not null)
        {
            foreach (var item in fileInfos) multipartFormContent.Add(new ByteArrayContent(await File.ReadAllBytesAsync(item.FullName)), item.Name, item.Name);
        }
        return await UploadFilesAsync(requestUrl, multipartFormContent, requestHeaders, cancellationToken);
    }

    /// <summary>
    /// Uploads multipart form data.
    /// </summary>
    /// <param name="requestUrl">The request URL.</param>
    /// <param name="multipartFormDataContent">The multipart form data content.</param>
    /// <param name="requestHeaders">The request headers.</param>
    /// <returns>The HTTP response message.</returns>
    public async Task<HttpResponseMessage> UploadFilesAsync(
        string requestUrl,
        MultipartFormDataContent? multipartFormDataContent,
        Dictionary<string, string>? requestHeaders,
        CancellationToken cancellationToken = default)
    {
        using var httpClient = CreateHttpClient();
        SetRequestHeaders(requestHeaders, httpClient);
        return await httpClient.PostAsync(GetRequestUri(requestUrl), multipartFormDataContent, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with optional body and headers.
    /// </summary>
    /// <param name="requestUrl">The request URL.</param>
    /// <param name="requestBody">The request body.</param>
    /// <param name="requestHeaders">The request headers.</param>
    /// <returns>The HTTP response message.</returns>
    public async Task<HttpResponseMessage> PostAsync(string requestUrl, object? requestBody = null, Dictionary<string, string>? requestHeaders = null,
         CancellationToken cancellationToken = default)
    {
        if (Not(string.IsNullOrWhiteSpace(requestUrl)) && requestUrl.StartsWith('/')) requestUrl = requestUrl.TrimStart('/');
        if (requestBody is MultipartFormDataContent) return await UploadFilesAsync(requestUrl, requestBody as MultipartFormDataContent, requestHeaders, cancellationToken);

        using var httpClient = CreateHttpClient();
        requestHeaders = SetRequestHeaders(requestHeaders, httpClient);

        StringContent? bodyContent = null;
        if (requestBody is not null)
        {
            if (IsXmlContentType(requestHeaders))
            {
                bodyContent = new StringContent(requestBody is string ? requestBody.ToString() : SerializeToXml(requestBody), Encoding.UTF8, requestHeaders["Content-Type"]);
            }
            else if (IsJsonContentType(requestHeaders))
            {
                bodyContent = new StringContent(requestBody.Serialize(), Encoding.UTF8, MediaTypeNames.Application.Json);
            }
            else if (IsJsonPatchContentType(requestHeaders))
            {
                bodyContent = new StringContent(requestBody.Serialize(), Encoding.UTF8, MediaTypeNames.Application.JsonPatch);
            }
            else
            {
                bodyContent = new StringContent(requestBody.ToString(), Encoding.UTF8, GetContentTypeValue(requestHeaders));
            }
            requestHeaders?.Remove(GetContentTypeHeaderName(requestHeaders));

        }

        return await httpClient.PostAsync(GetRequestUri(requestUrl), bodyContent, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sends a GET request with optional headers.
    /// </summary>
    /// <param name="requestUrl">The request URL.</param>
    /// <param name="requestHeaders">The request headers.</param>
    /// <returns>The HTTP response message.</returns>
    public async Task<HttpResponseMessage> GetAsync(string requestUrl, Dictionary<string, string>? requestHeaders = null, CancellationToken cancellationToken = default)
    {
        if (requestUrl.StartsWith('/')) requestUrl = requestUrl.TrimStart('/');
        using var httpClient = CreateHttpClient();
        SetRequestHeaders(requestHeaders, httpClient);
        return await httpClient.GetAsync(GetRequestUri(requestUrl), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sends an arbitrary HttpRequestMessage.
    /// </summary>
    /// <param name="httpRequestMessage">The HttpRequestMessage to send.</param>
    /// <returns>The HTTP response message.</returns>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
    {
        using var httpClient = CreateHttpClient();
        return await httpClient.SendAsync(httpRequestMessage, cancellationToken: cancellationToken);
    }
}