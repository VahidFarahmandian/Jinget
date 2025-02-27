using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Jinget.Handlers.ExternalServiceHandlers.Extensions;

/// <summary>
/// Provides extension methods for IServiceCollection to configure Jinget External Service Handlers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Jinget External Service Handler to the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="httpClientName">The named HttpClient client name. Defaults to "jinget-client".</param>
    /// <param name="bypassSslCertificateValidation">Indicates whether SSL certificate validation should be bypassed. Defaults to true.</param>
    public static void AddJingetExternalServiceHandler(this IServiceCollection services, string httpClientName = "jinget-client", bool bypassSslCertificateValidation = true)
    {
        // Registers JingetHttpClientFactory as a transient service.
        services.TryAddTransient<JingetHttpClientFactory>();

        // Configures a named HttpClient with a custom HttpClientHandler.
        services.AddHttpClient(httpClientName)
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                // Creates and configures HttpClientHandler using JingetHttpClientHandlerFactory.
                return JingetHttpClientHandlerFactory.Create(bypassSslCertificateValidation);
            });
    }
}