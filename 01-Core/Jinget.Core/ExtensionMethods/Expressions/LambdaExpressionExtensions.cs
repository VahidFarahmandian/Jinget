using Jinget.Core.Utilities.Expressions;
using System.Linq.Expressions;

namespace Jinget.Core.ExtensionMethods.Expressions
{
    public static class LambdaExpressionExtensions
    {
        /// <summary>
        /// Converts a <seealso cref="LambdaExpression"/> to string
        /// </summary>
        public static string Stringfy(this LambdaExpression expression)
        {
            if (expression == null)
                return null;
            ExpressionUtility.TryParseExpression(expression.Body, out var path);
            return path;
        }
    }
}