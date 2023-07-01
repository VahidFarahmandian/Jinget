using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Jinget.Core.Utilities;
using Newtonsoft.Json;

namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory
{
    public class HttpClientFactory
    {
        readonly HttpClient client;
        internal HttpClientFactory(string baseUri, bool ignoreSslErrors = false, Dictionary<string, string> headers = null)
        {
            if (ignoreSslErrors)
            {
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                };

                client = new HttpClient(httpClientHandler)
                {
                    BaseAddress = new Uri(baseUri)
                };
            }
            else
            {
                client = new HttpClient
                {
                    BaseAddress = new Uri(baseUri)
                };
            }
        }

        private Uri GetUrl(string url)
        {
            string baseUrl = client.BaseAddress.ToString().EndsWith("/") ? client.BaseAddress.ToString() : $"{client.BaseAddress}/";
            return new Uri($"{baseUrl}{url}".TrimEnd('/'));
        }
        private void SetHeaders(Dictionary<string, string>? headers)
        {
            if (headers == null)
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

        public async Task<HttpResponseMessage> PostAsync(string url, object content = null, Dictionary<string, string> headers = null)
        {
            if (url != "" && url.StartsWith("/"))
                throw new Jinget.Core.Exceptions.JingetException($"{nameof(url)} should not start with '/'");

            StringContent bodyContent = null;
            if (content != null)
            {
                if (headers != null && headers.Keys.Any(x => x == "Content-Type") && headers["Content-Type"].ToLower() != MediaTypeNames.Application.Json)
                {
                    if (headers["Content-Type"] == MediaTypeNames.Text.Xml || headers["Content-Type"] == MediaTypeNames.Application.Xml)
                    {
                        bodyContent = new StringContent(content is string ? content.ToString() : XmlUtility.SerializeToXml(content), Encoding.UTF8, headers["Content-Type"]);
                    }
                    else
                    {
                        bodyContent = new StringContent(content.ToString(), Encoding.UTF8, headers["Content-Type"]);
                    }
                }
                else
                {
                    bodyContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, MediaTypeNames.Application.Json);
                }
                headers?.Remove("Content-Type");
            }

            SetHeaders(headers);
            return await client.PostAsync(GetUrl(url), bodyContent);
        }

        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            if (url.StartsWith("/"))
                throw new Jinget.Core.Exceptions.JingetException($"{nameof(url)} should not start with '/'");
            SetHeaders(headers);
            return await client.GetAsync(GetUrl(url));
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message) => await client.SendAsync(message);
    }
}