using System;

namespace Jinget.Core.Attributes
{
    /// <summary>
    ///     Used for assigning custom configurations for entity properties
    /// </summary>
    public class EntityAttribute : Attribute
    {
        /// <summary> 
        /// Gets or sets a value indicating whether the entity is used as an elastic search index or not.
        /// </summary>
        /// <value><c>true</c> if [elastic search enabled]; otherwise, <c>false</c>.</value>
        public bool ElasticSearchEnabled { get; set; }
    }
}