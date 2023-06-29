using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory
{
    public abstract class ObjectFactory<T>
    {
        protected abstract T GetInstance(string baseUri, bool ignoreSslErrors = false, Dictionary<string, string> headers = null);

        public abstract Task<HttpResponseMessage> PostAsync(string baseUri, object content = null, bool ignoreSslErrors = false, Dictionary<string, string> headers = null);
        public abstract Task<HttpResponseMessage> SendAsync(string baseUri, HttpRequestMessage message, bool ignoreSslErrors = false);
        public abstract Task<HttpResponseMessage> GetAsync(string baseUri, string requestUri, bool ignoreSslErrors = false, Dictionary<string, string> headers = null);
    }
}