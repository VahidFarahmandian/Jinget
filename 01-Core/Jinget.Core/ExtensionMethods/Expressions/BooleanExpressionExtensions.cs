namespace Jinget.Core.ExtensionMethods.Expressions;

public static class BooleanExpressionExtensions
{
    /// <summary>
    /// Negate a boolean expression
    /// </summary>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
        => Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters[0]);

    /// <summary>
    /// Visit a boolean expression
    /// </summary>
    private static (Expression? LeftExpression, Expression? RightExpression) Visit<T>(
        Expression<Func<T, bool>> leftExpression,
        Expression<Func<T, bool>> rightExpression,
        ParameterExpression parameter)
    {
        ReplaceExpressionVisitor leftVisitor = new(leftExpression.Parameters[0], parameter);
        Expression? left = leftVisitor.Visit(leftExpression.Body);

        var rightVisitor = new ReplaceExpressionVisitor(rightExpression.Parameters[0], parameter);
        var right = rightVisitor.Visit(rightExpression.Body);

        return (left, right);
    }

    /// <summary>
    /// should combine conditions using AND operator. if any of expressions passed as null, then the other expression will be returned
    /// </summary>
    /// <param name="parameterName">parameter name used in expression. for example in x=>x.Id>0, parameterName is 'x'</param>
    public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2,
        string parameterName = "Param_0")
        => expr1.CreateBinaryExpression(expr2, parameterName, ExpressionType.AndAlso);

    /// <summary>
    /// should combine conditions using OR operator. if any of expressions passed as null, then the other expression will be returned
    /// </summary>
    /// <param name="parameterName">parameter name used in expression. for example in x=>x.Id>0, parameterName is 'x'</param>
    public static Expression<Func<T, bool>> OrElse<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2,
        string parameterName = "Param_0")
        => expr1.CreateBinaryExpression(expr2, parameterName, ExpressionType.OrElse);

    static Expression<Func<T, bool>> CreateBinaryExpression<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2,
        string parameterName = "Param_0",
        ExpressionType expressionType = ExpressionType.AndAlso)
    {
        if (expr1 is null) return expr2;
        if (expr2 is null) return expr1;
        var parameter = Expression.Parameter(typeof(T), parameterName);
        (Expression? LeftExpression, Expression? RightExpression) = Visit(expr1, expr2, parameter);
        if (LeftExpression == null || RightExpression == null)
        {
            throw new ArgumentNullException("Visit method returned null expression");
        }
        BinaryExpression binaryExpr;

        if (expressionType == ExpressionType.AndAlso)
            binaryExpr = Expression.AndAlso(LeftExpression, RightExpression);
        else if (expressionType == ExpressionType.OrElse)
            binaryExpr = Expression.OrElse(LeftExpression, RightExpression);
        else
            throw new Exception("Jinget Says: Only AndAlso and OrElse are supported");

        return Expression.Lambda<Func<T, bool>>(binaryExpr, parameter);
    }

}