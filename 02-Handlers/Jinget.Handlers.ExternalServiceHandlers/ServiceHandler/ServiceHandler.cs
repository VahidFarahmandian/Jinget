namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler;

public abstract class ServiceHandler<T> where T : new()
{
    public T Events { get; set; }

    protected HttpClientFactory HttpClientFactory { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ServiceHandler() => Events = new T();
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected ServiceHandler(string baseUri, bool ignoreSslErrors = false) : this() => HttpClientFactory = new HttpClientFactory(baseUri, ignoreSslErrors);

    protected ServiceHandler(string baseUri, TimeSpan timeout, bool ignoreSslErrors = false) : this() => HttpClientFactory = new HttpClientFactory(baseUri, timeout, ignoreSslErrors);
}