namespace Jinget.Core.Attributes;

/// <summary>
///     Used for assigning claims for actions
/// </summary>
public class ClaimAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Gets or sets the claim title.
    /// </summary>
    public required string Title { get; set; }
}