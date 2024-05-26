namespace Jinget.Blazor.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddJingetBlazor(
        this IServiceCollection services,
        TokenConfigModel? tokenConfig = null,
        bool addMudServices = true,
        bool addTokenAuthenticationStateProvider = true,
        bool addTokenStorageService = true)
    {
        if (addMudServices)
            services.AddMudServices();

        if (tokenConfig != null)
        {
            services.TryAddScoped<IJwtTokenService>(provider => new JwtTokenService(tokenConfig.Secret, tokenConfig.Expiration));

            if (addTokenStorageService)
            {
                services.TryAddScoped<ILocalStorageService, LocalStorageService>();
                services.TryAddScoped<ITokenStorageService>(
                    provider => new TokenStorageService(provider.GetRequiredService<ILocalStorageService>(), tokenConfig.Name));
            }
            if (addTokenAuthenticationStateProvider)
            {
                services.RemoveAll<AuthenticationStateProvider>();
                services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
                services.AddScoped<UserService>();
            }
        }
        return services;
    }
}