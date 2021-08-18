using System.Linq.Expressions;

namespace Jinget.Core.Utilities.Expressions
{
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node) => node == _oldValue ? _newValue : base.Visit(node);
    }
}
