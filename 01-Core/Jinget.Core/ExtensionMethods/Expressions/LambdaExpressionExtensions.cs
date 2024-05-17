using Jinget.Core.Utilities.Expressions;

namespace Jinget.Core.ExtensionMethods.Expressions;

public static class LambdaExpressionExtensions
{
    /// <summary>
    /// Converts a <seealso cref="LambdaExpression"/> to string
    /// </summary>
    public static string? Stringfy(this LambdaExpression expression)
    {
        if (expression is null)
            return null;
        var result = ExpressionUtility.TryParseExpression(expression.Body, out var path);
        return result ? path : null;
    }
}