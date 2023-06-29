using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.Core.Tests")]
namespace Jinget.Core.Utilities.Expressions
{
    /// <summary>
    /// Replaces one expression with another in given expression tree.
    /// </summary>
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        /// <param name="oldValue">The parameter is to be replaced by a new parameter(<paramref name="newValue"/>)</param>
        /// <param name="newValue">The parameter is to replace the old parameter(<paramref name="oldValue"/>)</param>
        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        /// <summary>
        /// Traverse the expression tree untill current node is equal to _oldValue, then it will swip _newValue with node
        /// </summary>
        /// <example>
        /// given _oldValue = x, _newValue = y, node = x=>x.Id>0
        /// then it will traverse this expression and replace the x with y
        /// so the result will be y=>y.Id>0
        /// </example>
        /// <param name="node">Expression tree that should be traversed</param>
        /// <returns></returns>
        public override Expression Visit(Expression node) => node == _oldValue ? _newValue : base.Visit(node);
    }
}
