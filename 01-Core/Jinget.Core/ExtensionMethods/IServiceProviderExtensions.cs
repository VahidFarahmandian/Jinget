namespace Jinget.Core.ExtensionMethods;

public static class IServiceProviderExtensions
{
    /// <summary>
    /// Find a registered service inside the service collection and return the matching type, if found
    /// </summary>
    /// <exception cref="InvalidOperationException">If there is no registered service of given type found, then <seealso cref="InvalidOperationException"/> will be thrown</exception>
    public static T? GetJingetService<T>(this IServiceProvider serviceProvider) where T : class
    {
        if (serviceProvider.CreateScope().ServiceProvider.GetRequiredService(typeof(T)) is T service)
            return service;
        return null;
    }
}