using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Jinget.Blazor.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static IServiceCollection AddJingetBlazor(this IServiceCollection services)
        {
            services.AddMudServices();

            return services;
        }
    }
}