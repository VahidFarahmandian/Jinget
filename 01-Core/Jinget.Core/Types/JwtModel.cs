﻿namespace Jinget.Core.Types;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class JwtModel
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public DateTime NotBefore { get; set; } = DateTime.Now;
    public int ExpirationInMinute { get; set; } = 5;
    public string TokenName { get; set; } = "jinget.token";
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

