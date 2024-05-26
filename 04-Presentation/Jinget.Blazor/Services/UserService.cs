namespace Jinget.Blazor.Services;

public class UserService(
    IAuthenticationService authenticationService,
    IJwtTokenService jwtTokenService,
    ITokenStorageService tokenStorageService)
{
    public event Action<ClaimsPrincipal?>? UserChanged;
    private ClaimsPrincipal? currentUser;

    public virtual ClaimsPrincipal? CurrentUser
    {
        get { return currentUser ?? new(); }
        set
        {
            if (currentUser != value)
            {
                currentUser = value;

                if (UserChanged is not null)
                {
                    UserChanged(currentUser);
                }
            }
        }
    }

    public virtual async Task<ClaimsPrincipal?> GetUserAsync()
    {
        var token = await tokenStorageService.GetTokenAsync();
        bool isTokenValid = false;
        try
        {
            isTokenValid = await JwtUtility.IsValidAsync(token);
        }
        catch { isTokenValid = false; }
        if (isTokenValid)
        {
            CurrentUser = new ClaimsPrincipal(await RefreshTokenAsync(token));
        }
        else
        {
            CurrentUser = null;
        }
        return CurrentUser;
    }

    async Task<ClaimsIdentity> RefreshTokenAsync(string oldToken)
    {
        var identity = new ClaimsIdentity(JwtUtility.Read(oldToken).Claims, "jwt");
        if (identity.Claims.Any())
        {
            string username = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            string[] roles = [identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value];
            var newToken = jwtTokenService.Generate(username, roles);

            await tokenStorageService.SetTokenAsync(newToken);
            return new ClaimsIdentity(JwtUtility.Read(newToken).Claims, "jwt");
        }
        return new();
    }

    /// <returns>If username and password are correct then returns true</returns>
    public virtual async Task<bool> LoginAsync(string username, string password)
    {
        var token = await authenticationService.LoginAsync(username, password);
        if (!string.IsNullOrWhiteSpace(token))
        {
            await tokenStorageService.SetTokenAsync(token);
            CurrentUser = await GetUserAsync();
        }

        return !string.IsNullOrWhiteSpace(token);
    }

    public virtual async Task LogoutAsync()
    {
        await tokenStorageService.RemoveTokenAsync();
    }
}
