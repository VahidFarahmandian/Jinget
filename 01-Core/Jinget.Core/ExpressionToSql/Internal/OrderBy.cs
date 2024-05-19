using Jinget.Core.ExtensionMethods.Expressions;
using Jinget.Core.Utilities.Expressions;

namespace Jinget.Core.ExpressionToSql.Internal;

/// <summary>
/// Provides the order by functionality used in query handling
/// </summary>
public class OrderBy : OrderBy<object>
{
    public OrderBy() : base() { }
    public OrderBy(string name) : base(name) { }
}
public class OrderBy<T>
{
    /// <summary>
    /// The order by expression
    /// </summary>
    public virtual Expression<Func<T, object>> Name { get; set; }

    public OrderBy() { }
    public OrderBy(string name) => Name = ExpressionUtility.ToExpression<T, object>(name, "x");

    /// <summary>
    /// Gets or sets the direction of sorting.
    /// <seealso cref="OrderByDirection"/>
    /// </summary>
    public OrderByDirection Direction { get; set; }

    /// <summary>
    /// Returns a <see cref="string" /> that represents the order by clause in T-SQL.
    /// </summary>
    public override string ToString()
    {
        StringBuilder orderByClause = new();
        orderByClause.Append('[');

        if (Name.Body is MemberExpression expression &&
            expression.Expression.NodeType != ExpressionType.Convert &&
            expression.Expression.NodeType != ExpressionType.Parameter)
            orderByClause.Append(Expression.Lambda(expression).Compile().DynamicInvoke());
        else
            orderByClause.Append(Name.Stringfy());

        orderByClause.Append(']');
        orderByClause.Append(' ');
        orderByClause.Append(Direction.GetDescription());

        return orderByClause.ToString();
    }
}