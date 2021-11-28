using System;

namespace Jinget.Core.Attributes
{
    /// <summary>
    ///     Manage Method Summary Attribute
    /// </summary>
    public class SummaryAttribute : Attribute
    {
        /// <summary>
        ///     provide method summary for using in access management
        /// </summary>
        /// <param name="description"></param>
        public SummaryAttribute(string description) => Description = description;

        /// <summary>
        ///     Action's description. this value will be shown to the end user
        /// </summary>
        public string Description { get; set; }
    }
}