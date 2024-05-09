namespace Jinget.Core.Contracts;

/// <summary>
/// Provide functionality for user context validation and management
/// Implements the <see cref="System.Security.Principal.IPrincipal" />
/// </summary>
/// <seealso cref="System.Security.Principal.IPrincipal" />
public interface IUserContext : IPrincipal
{
    /// <summary>
    /// check the access using claim attribute
    /// </summary>
    /// <param name="userIdentifier">The user identifier.</param>
    /// <param name="subSystemName">Name of the sub system.</param>
    /// <param name="apiName">Name of the API.</param>
    /// <param name="actionTitle">The action title.</param>
    /// <param name="claim">The claim.</param>
    Task<bool> HasAccessAsync(string userIdentifier, string subSystemName, string apiName, string actionTitle, string claim);

    /// <summary>
    /// Determines whether the token is valid for the given user or not.
    /// </summary>
    /// <param name="userIdentifier">The user identifier.</param>
    /// <param name="token">The token.</param>
    Task<bool> IsTokenValidAsync(string userIdentifier, string token);
}
