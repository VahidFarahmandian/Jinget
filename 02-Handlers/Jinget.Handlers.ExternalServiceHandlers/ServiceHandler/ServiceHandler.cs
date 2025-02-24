namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler;

public abstract class ServiceHandler<T> : ServiceHandler where T : new()
{
    public T Events { get; set; } = new T();

    protected ServiceHandler() { }
    protected ServiceHandler(string baseUri, bool ignoreSslErrors = false) : base(baseUri, ignoreSslErrors) { }

    protected ServiceHandler(string baseUri, TimeSpan timeout, bool ignoreSslErrors = false) : base(baseUri, timeout, ignoreSslErrors) { }
}
public abstract class ServiceHandler
{
    protected HttpClientFactory HttpClientFactory { get; set; }

    protected ServiceHandler() { }
    protected ServiceHandler(string baseUri, bool ignoreSslErrors = false) => HttpClientFactory = new HttpClientFactory(baseUri, ignoreSslErrors);

    protected ServiceHandler(string baseUri, TimeSpan timeout, bool ignoreSslErrors = false) => HttpClientFactory = new HttpClientFactory(baseUri, timeout, ignoreSslErrors);
}