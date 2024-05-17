using Jinget.Core.Types;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Jinget.Core.Utilities;

public class JwtUtility
{
    public static JwtSecurityToken? Read(string token, string scheme = "Bearer")
    {
        try
        {
            if (token.StartsWith($"{scheme} ", StringComparison.InvariantCultureIgnoreCase))
                token = token[7..];
            return new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Check if token is valid or not. This method only checks for the validity of lifetime, audience and issuer
    /// </summary>
    /// <param name="validAudiences">Expected list of audiences. It is expected that token was issued for one of these audiences</param>
    /// <param name="validissuer">Expected issuer. It is expected that token was issued for this issuer</param>
    /// <param name="minuteOffset">The given token is valid if it is valid for the next <paramref name="minuteOffset" /> minute(s)</param>
    public static async Task<bool> IsValidAsync(string token,
        IEnumerable<string>? validAudiences = null,
        string? validissuer = null,
        int minuteOffset = 5)
    {
        var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.FromMinutes(minuteOffset),

            ValidateLifetime = true,
            LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
            {
                var clonedParameters = validationParameters.Clone();
                clonedParameters.LifetimeValidator = null;
                Validators.ValidateLifetime(notBefore, expires, securityToken, clonedParameters);

                return true;
            },

            ValidateAudience = true,
            ValidAudiences = validAudiences,
            AudienceValidator = (IEnumerable<string>? audiences, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
            {
                if (validAudiences != null && validAudiences.Any())
                {
                    var clonedParameters = validationParameters.Clone();
                    clonedParameters.AudienceValidator = null;
                    Validators.ValidateAudience(audiences, securityToken, clonedParameters);
                }
                return true;
            },

            ValidateIssuer = true,
            ValidIssuer = validissuer,
            IssuerValidator = (string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
            {
                string resultIssuer = "";
                if (!string.IsNullOrWhiteSpace(validissuer))
                {
                    var clonedParameters = validationParameters.Clone();
                    clonedParameters.IssuerValidator = null;
                    resultIssuer = Validators.ValidateIssuer(issuer, securityToken, clonedParameters);
                }
                return resultIssuer;
            },

            ValidateIssuerSigningKey = false,
            SignatureValidator = (string token, TokenValidationParameters parameters) => new JwtSecurityToken(token)
        });
        if (result.Exception != null)
            throw result.Exception;

        return result.IsValid;
    }

    /// <summary>
    /// get given claim info stored inside the given token
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Claim>? GetClaim(string token, string claim)
    {
        try
        {
            var info = Read(token);
            if (info != null)
                return info.Claims.Where(x => x.Type == claim);
            else
                return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Create a new JWT token
    /// </summary>
    public static string Generate(string username, string[] roles, JwtModel options, int expirationInMinute = 15)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,username),
            new(ClaimTypes.Name,username),
        };
        foreach (var item in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, item));
        }
        return Generate(claims, options, expirationInMinute);
    }

    /// <summary>
    /// Create a new JWT token
    /// </summary>
    public static string Generate(IEnumerable<Claim> claims, JwtModel options, int expirationInMinute = 15)
    {
        if (string.IsNullOrWhiteSpace(options.SecretKey) || options.SecretKey.Length < 32)
        {
            throw new Exception("key should be a string with at least 32 chars");
        }
        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            options.NotBefore,
            DateTime.Now.AddMinutes(expirationInMinute),
            new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}