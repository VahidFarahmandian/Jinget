using Jinget.Core.ExtensionMethods.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Jinget.Core.ExpressionToSql.Internal
{
    public class Select<T, R> : Query
    {
        private readonly Expression<Func<T, R>> _select;
        private readonly int? _take;
        private readonly Table _table;

        public Where<T, R> Where(Expression<Func<T, bool>> predicate) => new(this, predicate);

        internal Select(Expression<Func<T, R>> select, int? take, Table table)
        {
            _select = select;
            _take = take;
            _table = table;
        }

        internal override (QueryBuilder query, Dictionary<string, object>? parameters) ToSql(QueryBuilder query)
        {
            if (_take.HasValue)
            {
                query.Take(_take.Value);
            }

            var type = _select.Parameters[0].Type;

            var expressions = GetExpressions(type, _select.Body);

            AddExpressions(expressions, type, query);

            query.AddTable(_table);

            return (query, null);
        }

        private static IEnumerable<Expression> GetExpressions(Type type, Expression body)
        {
            switch (body.NodeType)
            {
                case ExpressionType.New:
                    var n = (NewExpression)body;
                    return n.Arguments;
                case ExpressionType.Parameter:
                    var propertyInfos = type.GetWritableProperties();
                    return propertyInfos.Values.Select(pi => Expression.Property(body, pi));
                default:
                    return new[] { body };
            }
        }

        private static void AddExpressions(IEnumerable<Expression> es, Type t, QueryBuilder qb)
        {
            foreach (var e in es)
            {
                AddExpression(e, t, qb);
                qb.AddSeparator();
            }
            qb.Remove(); // Remove last comma
        }

        private static void AddExpression(Expression e, Type t, QueryBuilder qb)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Constant:
                    var c = (ConstantExpression)e;
                    qb.AddValue(c.Value);
                    break;
                case ExpressionType.MemberAccess:
                    var m = (MemberExpression)e;
                    AddExpression(m, t, qb);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void AddExpression(MemberExpression m, Type t, QueryBuilder qb)
        {
            if (m.Member.DeclaringType == t)
            {
                qb.AddAttribute(m.Member.Name);
            }
            else
            {
                qb.AddParameter(m.Member.Name);
            }
        }
    }
}