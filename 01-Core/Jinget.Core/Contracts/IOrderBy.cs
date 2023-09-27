using System.Collections.Generic;
using Jinget.Core.ExpressionToSql.Internal;

namespace Jinget.Core.Contracts
{
    /// <summary>
    /// Used for supporting sorting mechanism in elastic search query handling
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOrderBy<T>
    {
        /// <summary>
        /// Gets or sets the order by clause.
        /// </summary>
        /// <value>The order by.</value>
        T OrderBy { get; set; }
    }

    /// <summary>
    /// Used for supporting sorting mechanism in dapper query handling
    /// </summary>
    public interface IOrderBy : IOrderBy<List<OrderBy>>
    {
    }
}
