using System;
using System.Linq.Expressions;
using System.Text;
using Jinget.Core.Enumerations;
using Jinget.Core.ExtensionMethods.Enums;
using Jinget.Core.ExtensionMethods.Expressions;
using Jinget.Core.Utilities.Expressions;

namespace Jinget.Core.ExpressionToSql.Internal;

/// <summary>
/// Provides the order by functionality used in query handling
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrderBy
{
    public OrderBy()
    {

    }
    public OrderBy(string name) => Name = ExpressionUtility.ToExpression<object>(name, "x");

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
        StringBuilder orderByClause = new();
        orderByClause.Append('[');

        if (Name.Body is MemberExpression expression && expression.Expression.NodeType != ExpressionType.Convert)
            orderByClause.Append(Expression.Lambda(expression).Compile().DynamicInvoke());
        else
            orderByClause.Append(Name.Stringfy());

        orderByClause.Append(']');
        orderByClause.Append(' ');
        orderByClause.Append(Direction.GetDescription());

        return orderByClause.ToString();
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

