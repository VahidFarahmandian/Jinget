using System;
using System.Linq.Expressions;

namespace Jinget.Core.Contracts
{
    /// <summary>
    /// Used for supporting row restriction mechanism in query handling.
    /// RowRestrictions are filters which should be automatically applied to the queries regardless of their FILTER properties.
    /// The final WHERE clause is made up: (RowRestrictions) AND (Filter)
    /// </summary>
    /// <typeparam name="TModelType">The type of the t model type.</typeparam>
    public interface IRowRestriction<TModelType>
    {
        /// <summary>
        /// Gets or sets the row restrictions.
        /// </summary>
        /// <value>The row restrictions.</value>
        Expression<Func<TModelType, bool>> RowRestrictions { get; set; }
    }
}
