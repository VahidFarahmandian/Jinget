using Jinget.Core.Utilities;
using Jinget.Handlers.ExternalServiceHandlers.ServiceHandler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Jinget.Handlers.ExternalServiceHandlers.DefaultServiceHandler
{
    public class JingetServiceHandler<TResponseModel> : ServiceHandler<JingetServiceHandlerEvents<TResponseModel>> where TResponseModel : class, new()
    {
        private async Task<TResponseModel> ProcessTask(Func<Task<HttpResponseMessage>> task)
        {
            TResponseModel responseModel = null;
            try
            {
                var response = await task();
                Events.OnServiceCalled(response);
                response.EnsureSuccessStatusCode();

                string rawResponse = await response.Content.ReadAsStringAsync();
                Events.OnRawResponseReceived(rawResponse);

                switch (response.Content.Headers.ContentType.MediaType)
                {
                    case MediaTypeNames.Application.Json:
                        responseModel = JsonConvert.DeserializeObject<TResponseModel>(rawResponse);
                        break;
                    case MediaTypeNames.Application.Xml:
                    case MediaTypeNames.Text.Xml:
                        responseModel = XmlUtility.DeserializeXmlDescendantsFirst<TResponseModel>(rawResponse);
                        break;
                    default:
                        break;
                }

                Events.OnResponseDeserialized(responseModel);
            }
            catch (Exception ex)
            {
                Events.OnExceptionOccurred(ex);
            }
            return responseModel;

        }
        public async Task<TResponseModel> GetAsync(string baseUri, string requestUri, bool ignoreSslErrors = false, Dictionary<string, string> headers = null) => await ProcessTask(async () => await HttpClientFactory.GetAsync(baseUri, requestUri, ignoreSslErrors, headers));

        public async Task<TResponseModel> PostAsync(string baseUri, object content = null, bool ignoreSslErrors = false, Dictionary<string, string> headers = null) => await ProcessTask(async () => await HttpClientFactory.PostAsync(baseUri, content, ignoreSslErrors, headers));

        public async Task<TResponseModel> SendAsync(string baseUri, HttpRequestMessage message, bool ignoreSslErrors = false) => await ProcessTask(async () => await HttpClientFactory.SendAsync(baseUri, message, ignoreSslErrors));
    }
}
