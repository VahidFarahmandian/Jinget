namespace Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory;

/// <summary>
/// A static factory for creating HttpClientHandler instances.
/// </summary>
public static class JingetHttpClientHandlerFactory
{
    /// <summary>
    /// Creates an HttpClientHandler with optional SSL error ignoring.
    /// </summary>
    /// <param name="ignoreSslErrors">True to ignore SSL errors, false otherwise.</param>
    /// <returns>A new HttpClientHandler instance.</returns>
    public static HttpClientHandler Create(bool ignoreSslErrors)
    {
        if (ignoreSslErrors)
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
        }
        else
        {
            return new HttpClientHandler();
        }
    }
}