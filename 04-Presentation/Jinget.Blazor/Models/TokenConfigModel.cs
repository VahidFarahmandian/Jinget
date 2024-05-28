namespace Jinget.Blazor.Models;

public class TokenConfigModel
{
    /// <summary>
    /// secret key used to generate the token
    /// </summary>
    public string Secret { get; set; }

    /// <summary>
    /// token expiration in minute. Default value is 5 minutes
    /// </summary>
    public int ExpirationInMinute { get; set; } = 5;

    /// <summary>
    /// localstorage item name which holds the token. Default name is 'jinget.token'
    /// </summary>
    public string TokenName { get; set; } = "jinget.token";
}