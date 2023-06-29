using Microsoft.AspNetCore.Authorization;

namespace Jinget.Core.Attributes
{
    /// <summary>
    ///     Used for assigning claims for actions
    /// </summary>
    public class ClaimAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Gets or sets the claim title.
        /// </summary>
        public string Title { get; set; }
    }
}