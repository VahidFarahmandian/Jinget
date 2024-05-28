namespace Jinget.Blazor.Services.Contracts;

public interface IJwtTokenService
{
    string Generate(string username, string[] roles, IEnumerable<Claim>? otherClaims = null);
}
