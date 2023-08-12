using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Jinget.Core.Utilities;
using Jinget.Core.Utilities.Http;
using Newtonsoft.Json;

namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory
{
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

        private Uri GetUrl(string url) => new Uri($"{client.BaseAddress.ToString().TrimEnd('/')}/{url}".TrimEnd('/'));

#nullable enable
        private void SetHeaders(Dictionary<string, string>? headers)
#nullable disable
        {
            if (headers is null)
                return;
            foreach (var header in headers)
            {
                if (header.Key == "Authorization")
                {
                    if (header.Value.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", header.Value[7..]);
                    else if (header.Value.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", header.Value[6..]);
                    else if (header.Value.StartsWith("Digest ", StringComparison.InvariantCultureIgnoreCase))
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Digest", header.Value[7..]);
                }
                else
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        public async Task<HttpResponseMessage> UploadFileAsync(string url, List<FileInfo> files = null, Dictionary<string, string> headers = null)
        {
            using var multipartFormContent = new MultipartFormDataContent();
            foreach (var item in files)
            {
                //bool isMimeTypeAvailable = MimeTypeMap.TryGetMimeType(item.Name, out var mimeType);

                var st = new ByteArrayContent(await File.ReadAllBytesAsync(item.FullName));
                //st.Headers.ContentType = new MediaTypeHeaderValue(isMimeTypeAvailable ? mimeType : MediaTypeNames.Application.Octet);
                multipartFormContent.Add(st, "file", item.Name);
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
            if (!string.IsNullOrWhiteSpace(url) && url.StartsWith('/'))
                url = url.TrimStart('/');
            if (content is MultipartFormDataContent)
            {
                return await UploadFileAsync(url, content as MultipartFormDataContent, headers);
            }

            StringContent bodyContent = null;
            if (content != null)
            {
                if (HeaderUtility.IsXmlContentType(headers))
                {
                    bodyContent = new StringContent(content is string ? content.ToString() : XmlUtility.SerializeToXml(content), Encoding.UTF8, headers["Content-Type"]);
                }
                else if (HeaderUtility.IsJsonContentType(headers))
                {
                    bodyContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, MediaTypeNames.Application.Json);
                }
                else
                {
                    bodyContent = new StringContent(content.ToString(), Encoding.UTF8, HeaderUtility.GetContentTypeValue(headers));
                }
                headers?.Remove(HeaderUtility.GetContentTypeHeaderName(headers));
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
}