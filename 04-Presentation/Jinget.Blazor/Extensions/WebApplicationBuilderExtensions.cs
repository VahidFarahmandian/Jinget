namespace Jinget.Blazor.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddJingetBlazor(
        this IServiceCollection services,
        TokenConfigModel? tokenConfig = null,
        bool addLocalStorageService = true,
        bool AddUserService = true)
    {
        services.AddMudServices();

        if (AddUserService)
            services.AddScoped<UserService>();

        if (addLocalStorageService)
            services.TryAddScoped<ILocalStorageService, LocalStorageService>();

        if (tokenConfig != null)
        {
            services.TryAddScoped<IJwtTokenService>(provider => new JwtTokenService(tokenConfig.Secret, tokenConfig.Expiration));
            services.TryAddScoped<ITokenStorageService>(
                provider => new TokenStorageService(provider.GetRequiredService<ILocalStorageService>(), tokenConfig.Name));
            services.RemoveAll<AuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
        }
        return services;
    }
}