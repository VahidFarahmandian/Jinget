namespace Jinget.Core.Utilities;

public static class GenericsUtility
{
    // --------------------------------------------------------------------------
    // Case 1: Direct value comparison (C# only, no LINQ translation)
    // Example: AreEqual(1, 2) → false
    // --------------------------------------------------------------------------
    public static bool AreEqual<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }

    // --------------------------------------------------------------------------
    // Case 2: Compare entity property to a constant value (LINQ-compatible)
    // Example: x => AreEqual(x.Id, 2) → translates to SQL: WHERE Id = 2
    // --------------------------------------------------------------------------
    public static Expression<Func<T, bool>> AreEqual<T, TValue>(
        Expression<Func<T, TValue>> propertySelector,
        TValue value)
    {
        var param = propertySelector.Parameters[0];
        var property = propertySelector.Body;
        var constant = Expression.Constant(value, typeof(TValue));
        var equals = Expression.Equal(property, constant);
        return Expression.Lambda<Func<T, bool>>(equals, param);
    }

    // --------------------------------------------------------------------------
    // Case 3: Compare two properties of the same entity (LINQ-compatible)
    // Example: x => AreEqual(x.Name, x.DisplayName) → WHERE Name = DisplayName
    // --------------------------------------------------------------------------
    public static Expression<Func<T, bool>> AreEqual<T, TValue>(
        Expression<Func<T, TValue>> propertySelector1,
        Expression<Func<T, TValue>> propertySelector2)
    {
        var param = Expression.Parameter(typeof(T), "x");

        // Replace parameters in both expressions with the new shared parameter
        var left = ReplaceParameter(propertySelector1.Body, propertySelector1.Parameters[0], param);
        var right = ReplaceParameter(propertySelector2.Body, propertySelector2.Parameters[0], param);

        var equals = Expression.Equal(left, right);
        return Expression.Lambda<Func<T, bool>>(equals, param);
    }

    // --------------------------------------------------------------------------
    // Case 4: Compare a constant to an entity property (LINQ-compatible)
    // Example: x => AreEqual(5, x.Value) → WHERE 5 = Value
    // --------------------------------------------------------------------------
    public static Expression<Func<T, bool>> AreEqual<T, TValue>(
        TValue value,
        Expression<Func<T, TValue>> propertySelector)
    {
        var param = propertySelector.Parameters[0];
        var constant = Expression.Constant(value, typeof(TValue));
        var property = propertySelector.Body;
        var equals = Expression.Equal(constant, property);
        return Expression.Lambda<Func<T, bool>>(equals, param);
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression oldParam, ParameterExpression newParam) => new ParameterReplacerVisitor(oldParam, newParam).Visit(expression);

    // ExpressionVisitor to replace parameters
    private class ParameterReplacerVisitor(ParameterExpression oldParam, ParameterExpression newParam) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => node == oldParam ? newParam : base.VisitParameter(node);
    }
}
