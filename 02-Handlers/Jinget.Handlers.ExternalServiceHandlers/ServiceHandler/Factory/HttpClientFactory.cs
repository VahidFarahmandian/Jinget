using Jinget.Core.ExtensionMethods;

namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory;

public class HttpClientFactory
{
    readonly HttpClient client;
    internal HttpClientFactory(string baseUri, bool ignoreSslErrors = false)
    {
        if (ignoreSslErrors)
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };

            client = new HttpClient(httpClientHandler);
        }
        else
        {
            client = new HttpClient();
        }
        client.BaseAddress = new Uri(baseUri);
    }

    internal HttpClientFactory(string baseUri, TimeSpan timeout, bool ignoreSslErrors = false) : this(baseUri, ignoreSslErrors) => client.Timeout = timeout;

    private Uri GetUrl(string url) => new($"{client.BaseAddress?.ToString().TrimEnd('/')}/{url}".TrimEnd('/'));

#nullable enable
    private void SetHeaders(Dictionary<string, string>? headers)
#nullable disable
    {
        if (headers is null)
            return;
        foreach (var header in headers)
        {
            if (Not(header.Key.Equals("Authorization", StringComparison.InvariantCultureIgnoreCase)))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                continue;
            }

            if (header.Value.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", header.Value[7..]);
            else if (header.Value.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", header.Value[6..]);
            else if (header.Value.StartsWith("Digest ", StringComparison.InvariantCultureIgnoreCase))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Digest", header.Value[7..]);
        }
    }

    public async Task<HttpResponseMessage> UploadFileAsync(string url, List<FileInfo> files = null, Dictionary<string, string> headers = null)
    {
        using var multipartFormContent = new MultipartFormDataContent();
        foreach (var item in files)
        {
            var st = new ByteArrayContent(await File.ReadAllBytesAsync(item.FullName));
            multipartFormContent.Add(st, item.Name, item.Name);
        }
        return await UploadFileAsync(url, multipartFormContent, headers);
    }
    public async Task<HttpResponseMessage> UploadFileAsync(string url, MultipartFormDataContent multipartFormData, Dictionary<string, string> headers = null)
    {
        SetHeaders(headers);
        return await client.PostAsync(GetUrl(url), multipartFormData);
    }

    public async Task<HttpResponseMessage> PostAsync(string url, object content = null, Dictionary<string, string> headers = null)
    {
        if (Not(string.IsNullOrWhiteSpace(url)) && url.StartsWith('/'))
            url = url.TrimStart('/');
        if (content is MultipartFormDataContent)
        {
            return await UploadFileAsync(url, content as MultipartFormDataContent, headers);
        }

        StringContent bodyContent = null;
        if (content is not null)
        {
            if (IsXmlContentType(headers))
            {
                bodyContent = new StringContent(content is string ? content.ToString() : SerializeToXml(content), Encoding.UTF8, headers["Content-Type"]);
            }
            else if (IsJsonContentType(headers))
            {
                bodyContent = new StringContent(content.Serialize(), Encoding.UTF8, MediaTypeNames.Application.Json);
            }
            else
            {
                bodyContent = new StringContent(content.ToString(), Encoding.UTF8, GetContentTypeValue(headers));
            }
            headers?.Remove(GetContentTypeHeaderName(headers));
        }

        SetHeaders(headers);
        return await client.PostAsync(GetUrl(url), bodyContent);
    }

    public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null)
    {
        if (url.StartsWith('/'))
            url = url.TrimStart('/'); SetHeaders(headers);

        return await client.GetAsync(GetUrl(url));
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message) => await client.SendAsync(message);
}