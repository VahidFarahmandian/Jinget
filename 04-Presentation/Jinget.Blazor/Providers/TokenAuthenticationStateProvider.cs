using Jinget.Blazor.Services;

namespace Jinget.Blazor.Providers;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly UserService service;

    public TokenAuthenticationStateProvider(UserService service)
    {
        this.service = service;
        service.UserChanged += (newUser) =>
        {
            newUser = newUser ?? new();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(newUser)));

        };
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await service.GetUserAsync() ?? new();
        return await Task.FromResult(new AuthenticationState(user));
    }
}