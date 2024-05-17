namespace Jinget.Blazor.Services;

public class JwtTokenService(string secret, int expirationInMinute) : IJwtTokenService
{
    public string Generate(string username, string[] roles)
    {
        var jwtOptions = new Core.Types.JwtModel
        {
            SecretKey = secret
        };
        List<Claim> claims =
        [
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, username)
        ];
        foreach (string value in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, value));
        }
        return JwtUtility.Generate(claims, jwtOptions, expirationInMinute);
    }
}
