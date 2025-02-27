using Microsoft.Extensions.DependencyInjection;

namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler;

/// <summary>
/// Abstract base class for service handlers, providing HttpClientFactory.
/// </summary>
public abstract class ServiceHandlerBase
{
    /// <summary>
    /// Gets or sets the HttpClientFactory used for making HTTP requests.
    /// </summary>
    protected JingetHttpClientFactory HttpClientFactory { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceHandlerBase"/> class with the specified base URI, timeout, and SSL error handling.
    /// </summary>
    /// <param name="baseUri">The base URI for the HttpClient.</param>
    /// <param name="timeout">The timeout for HTTP requests.</param>
    /// <param name="ignoreSslErrors">A value indicating whether to ignore SSL errors.</param>
    protected ServiceHandlerBase(IServiceProvider serviceProvider, string baseUri, string clientName = "jinget-client", TimeSpan? timeout = null)
    {
        HttpClientFactory = serviceProvider.GetRequiredService<JingetHttpClientFactory>();
        HttpClientFactory.clientName = clientName;
        HttpClientFactory.baseUri = baseUri;
        HttpClientFactory.timeout = timeout;
    }
}

/// <summary>
/// Abstract base class for service handlers, providing HttpClientFactory and event management.
/// </summary>
/// <typeparam name="T">The type of events associated with the service handler.</typeparam>
public abstract class ServiceHandler<T> : ServiceHandlerBase where T : new()
{
    /// <summary>
    /// Gets or sets the events associated with the service handler.
    /// </summary>
    public T Events { get; set; } = new T();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceHandler{T}"/> class with the specified base URI, timeout, and SSL error handling.
    /// </summary>
    /// <param name="baseUri">The base URI for the HttpClient.</param>
    /// <param name="timeout">The timeout for HTTP requests.</param>
    /// <param name="ignoreSslErrors">A value indicating whether to ignore SSL errors.</param>
    protected ServiceHandler(IServiceProvider serviceProvider, string baseUri, string clientName = "jinget-client", TimeSpan? timeout = null) : base(serviceProvider, baseUri, clientName, timeout) { }
}