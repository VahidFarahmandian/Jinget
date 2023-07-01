using Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory;
using System.Collections.Generic;

namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler
{
    public abstract class ServiceHandler<T> where T : new()
    {
        public T Events { get; set; }

        protected HttpClientFactory HttpClientFactory { get; set; }

        protected ServiceHandler(string baseUri, bool ignoreSslErrors = false, Dictionary<string, string> headers = null)
        {
            Events = new T();
            HttpClientFactory = new HttpClientFactory(baseUri, ignoreSslErrors, headers);
        }
    }
}