using System;

namespace Jinget.Core.Attributes
{
    /// <summary>
    ///     Manage Method Summary Attribute
    /// </summary>
    /// <remarks>
    ///     provide method summary for using in access management
    /// </remarks>
    /// <param name="description"></param>
    public class SummaryAttribute(string description) : Attribute
    {

        /// <summary>
        ///     Action's description. this value will be shown to the end user
        /// </summary>
        public string Description { get; set; } = description;
    }
}