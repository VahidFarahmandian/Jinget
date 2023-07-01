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
    public class HttpClientFactory : ObjectFactory<HttpClient>
    {
        protected override HttpClient GetInstance(string baseUri, bool ignoreSslErrors = false, Dictionary<string, string> headers = null)
        {
            HttpClient client;
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

            client.DefaultRequestHeaders.Clear();

            if (headers == null)
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            }
            else
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return client;
        }

        public override async Task<HttpResponseMessage> PostAsync(string baseUri, object content = null, bool ignoreSslErrors = false, Dictionary<string, string> headers = null)
        {
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
            return await GetInstance(baseUri, ignoreSslErrors, headers).PostAsync(baseUri, bodyContent);
        }

        public override async Task<HttpResponseMessage> GetAsync(string baseUri, string requestUri, bool ignoreSslErrors = false, Dictionary<string, string> headers = null) => await GetInstance(baseUri, ignoreSslErrors, headers).GetAsync(baseUri + requestUri);

        public override async Task<HttpResponseMessage> SendAsync(string baseUri, HttpRequestMessage message, bool ignoreSslErrors = false) => await GetInstance(baseUri, ignoreSslErrors).SendAsync(message);
    }
}