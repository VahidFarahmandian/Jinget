using Microsoft.AspNetCore.Builder;

namespace Jinget.ExceptionHandler.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Add logging required middlewares to pipeline
    /// </summary>
    public static IApplicationBuilder UseJingetExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        app.Use(async (ctx, next) =>
        {
            var baseSetting = ctx.RequestServices.GetJingetService<BaseSettingModel>();
            await next();
            if (baseSetting != null && baseSetting.Handle4xxResponses)
            {
                if (ctx.Response.StatusCode is >= 400 and < 500 && ctx.Response.StatusCode != 429)
                {
                    throw new HttpRequestException(ReasonPhrases.GetReasonPhrase(ctx.Response.StatusCode), null, (System.Net.HttpStatusCode?)ctx.Response.StatusCode);
                }
            }
        });

        return app;
    }
}