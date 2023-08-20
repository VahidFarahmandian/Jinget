using Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory;
using System;

namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler
{
    public abstract class ServiceHandler<T> where T : new()
    {
        public T Events { get; set; }

        protected HttpClientFactory HttpClientFactory { get; set; }

        private ServiceHandler()
        {
            Events = new T();
        }
        protected ServiceHandler(string baseUri, bool ignoreSslErrors = false) : this()
        {
            HttpClientFactory = new HttpClientFactory(baseUri, ignoreSslErrors);
        }

        protected ServiceHandler(string baseUri, TimeSpan timeout, bool ignoreSslErrors = false) : this()
        {
            HttpClientFactory = new HttpClientFactory(baseUri, timeout, ignoreSslErrors);
        }
    }
}