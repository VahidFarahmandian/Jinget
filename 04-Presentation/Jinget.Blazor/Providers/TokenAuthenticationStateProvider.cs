namespace Jinget.Blazor.Providers;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly UserService _service;

    public TokenAuthenticationStateProvider(UserService service)
    {
        _service = service;
        _service.UserChanged += (newUser) =>
        {
            newUser = newUser ?? new();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(newUser)));
        };
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        => await Task.FromResult(new AuthenticationState(await _service.GetUserAsync() ?? new()));
}