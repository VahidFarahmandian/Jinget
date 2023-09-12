﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Jinget.Core.Utilities
{
    public class JwtUtility
    {
        public static JwtSecurityToken Read(string token, string scheme = "Bearer")
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
            IEnumerable<string> validAudiences = null,
            string validissuer = null,
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
                AudienceValidator = (IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
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
    }
}