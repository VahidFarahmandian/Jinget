namespace Jinget.Blazor.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddJingetBlazor(
        this IServiceCollection services,
        TokenConfigModel? tokenConfig = null,
        bool addMudServices = true)
    {
        if (addMudServices)
            services.AddMudServices();

        if (tokenConfig != null)
        {
            services.TryAddScoped<IJwtTokenService>(provider => new JwtTokenService(tokenConfig.Secret, tokenConfig.Expiration));
            services.TryAddScoped<ITokenStorageService>(
                provider => new TokenStorageService(provider.GetRequiredService<ILocalStorageService>(), tokenConfig.Name));

            services.RemoveAll<AuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
            services.AddScoped<UserService>();
            services.TryAddScoped<ILocalStorageService, LocalStorageService>();
        }
        return services;
    }
}