namespace Jinget.Core.DiScanner.Internal;

internal static class ServiceCollectionExtensions
{
    public static bool HasRegistration(this IServiceCollection services, Type serviceType) => services.Any(x => x.ServiceType == serviceType);
}
