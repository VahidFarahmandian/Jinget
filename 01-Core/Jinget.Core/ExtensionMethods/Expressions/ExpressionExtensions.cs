namespace Jinget.Core.ExtensionMethods.Expressions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Transform anonymous expression to non-anonymous expression
    /// This method is useful wherever the class's default constructor is not accessible due to its access level
    /// but you need to use this constructor to define your expression
    /// </summary>
    public static Expression<Func<T, T>> Transform<T>(this Expression<Func<T, object>> source)
        => Expression.Lambda<Func<T, T>>(ExpressionUtility.Transform(source.Body, typeof(T)), source.Parameters);
}