using Microsoft.AspNetCore.Authorization;

namespace Jinget.Core.Attributes
{
    /// <summary>
    ///     Used for assigning claims for actions
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class ClaimAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Gets or sets the claim title.
        /// </summary>
        public string Title { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}