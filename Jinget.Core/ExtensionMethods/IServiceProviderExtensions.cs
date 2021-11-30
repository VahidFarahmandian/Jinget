using System;
using Microsoft.Extensions.DependencyInjection;

namespace Jinget.Core.ExtensionMethods
{
    public static class IServiceProviderExtensions
    {
        public static T GetJingetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            if (serviceProvider.CreateScope().ServiceProvider.GetRequiredService(typeof(T)) is T service)
                return service;
            return null;
        }
    }
}