using System;
using System.Linq.Expressions;
using System.Text;
using Jinget.Core.Enumerations;
using Jinget.Core.ExtensionMethods.Enums;
using Jinget.Core.ExtensionMethods.Expressions;

namespace Jinget.Core.ExpressionToSql.Internal
{
    /// <summary>
    /// Provides the order by functionality used in query handling
    /// </summary>
    public class OrderBy
    {
        /// <summary>
        /// The order by expression
        /// </summary>
        public Expression<Func<object, object>> Name { get; set; }

        /// <summary>
        /// Gets or sets the direction of sorting.
        /// <seealso cref="OrderByDirection"/>
        /// </summary>
        public OrderByDirection Direction { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the order by clause in T-SQL.
        /// </summary>
        public override string ToString()
        {
            StringBuilder orderByClause = new StringBuilder();
            orderByClause.Append("[");

            if (Name.Body is MemberExpression expression && expression.Expression.NodeType != ExpressionType.Convert)
                orderByClause.Append(Expression.Lambda(expression).Compile().DynamicInvoke());
            else
                orderByClause.Append(Name.Stringfy());

            orderByClause.Append("]");
            orderByClause.Append(" ");
            orderByClause.Append(Direction.GetDescription());

            return orderByClause.ToString();
        }
    }
}
