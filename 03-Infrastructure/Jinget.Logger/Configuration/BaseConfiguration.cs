using Jinget.Logger.Configuration.Middlewares;
using Jinget.Logger.ExceptionHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Jinget.Logger.Configuration
{
    internal static class BaseConfiguration
    {
        internal static void ConfigurePrerequisites(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddScoped<IExceptionHandler<LogRequestMiddleware>, ExceptionHandler<LogRequestMiddleware>>();
        }
    }
}