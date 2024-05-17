namespace Jinget.Blazor.Providers;

public class TokenAuthenticationStateProvider(
    ITokenStorageService tokenStorage,
    IJwtTokenService tokenService) : AuthenticationStateProvider
{
    public void StateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStorage.GetTokenAsync().ConfigureAwait(false);
        ClaimsIdentity? identity = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(token) && await JwtUtility.IsValidAsync(token).ConfigureAwait(false))
            {
                identity = new ClaimsIdentity(JwtUtility.Read(token).Claims, "jwt");
                await RefreshTokenAsync(identity).ConfigureAwait(false);
            }
        }
        catch { }
        finally
        {
            identity ??= new ClaimsIdentity();
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    async Task RefreshTokenAsync(ClaimsIdentity identity)
    {
        if (identity.Claims.Any())
        {
#pragma warning disable CS8602
            string username = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            string[] roles = [identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value];
#pragma warning restore CS8602
            var newToken = tokenService.Generate(username, roles);
            await tokenStorage.SetTokenAsync(newToken).ConfigureAwait(false);
        }
    }
}