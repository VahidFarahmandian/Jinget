using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Jinget.Core.Exceptions;

namespace Jinget.Core.ExpressionToSql.Internal
{
    public class Where<T, R> : Query
    {
        private readonly Select<T, R> _select;
        private readonly Expression<Func<T, bool>> _where;

        internal Where(Select<T, R> select, Expression<Func<T, bool>> where)
        {
            _select = select;
            _where = where;
        }

        internal override (QueryBuilder query, Dictionary<string, object> parameters) ToSql(QueryBuilder query)
        {
            _select.ToSql(query);

            var i = 1;

            if (_where is null)
                return (query, new Dictionary<string, object>());

            var where = Recurse(ref i, _where.Body, true);
            query.AppendCondition(@where.ToString());
            return (query, @where.Parameters);
        }

        private WherePart Recurse(ref int i, Expression expression, bool isUnary = false, string? prefix = null, string? postfix = null)
        {
            if (expression is UnaryExpression unary)
            {
                return WherePart.Concat(NodeTypeToString(unary.NodeType), Recurse(ref i, unary.Operand, true));
            }
            if (expression is BinaryExpression binary)
            {
                return WherePart.Concat(Recurse(ref i, binary.Left), NodeTypeToString(binary.NodeType), Recurse(ref i, binary.Right));
            }
            if (expression is ConstantExpression constant)
            {
                var value = constant.Value;
                if (value is string @string)
                {
                    value = prefix + @string + postfix;
                }
                if (value is bool && isUnary)
                {
                    return WherePart.Concat(WherePart.IsParameter(i++, value), "=", WherePart.IsSql("1"));
                }
                return WherePart.IsParameter(i++, value);
            }
            if (expression is MemberExpression member)
            {
                switch (member.Member)
                {
                    case PropertyInfo property:
                        if (isUnary && member.Type == typeof(bool))
                        {
                            return WherePart.Concat(Recurse(ref i, member), "=", WherePart.IsParameter(i++, true));
                        }

                        if (((MemberExpression)expression).Expression.NodeType == ExpressionType.Constant)
                        {
                            goto case_ConstantExpression;
                        }
                        return WherePart.IsSql("[" + property.Name + "]");
                    case FieldInfo _:
                    case_ConstantExpression:
                        var value = GetValue(member);
                        if (value is string @string)
                        {
                            value = prefix + @string + postfix;
                        }

                        return WherePart.IsParameter(i++, value);
                }

                throw new JingetException($"Jinget Says: Expression does not refer to a property or field: {member}");
            }
            if (expression is MethodCallExpression methodCall)
            {
                // %xxx LIKE queries:
                if (methodCall.Method == typeof(string).GetMethod("StartsWith", [typeof(string)]))
                {
                    return WherePart.Concat(Recurse(ref i, methodCall.Object), "LIKE", Recurse(ref i, methodCall.Arguments[0], postfix: "%"));
                }
                // xxx% LIKE queries:
                if (methodCall.Method == typeof(string).GetMethod("EndsWith", [typeof(string)]))
                {
                    return WherePart.Concat(Recurse(ref i, methodCall.Object), "LIKE", Recurse(ref i, methodCall.Arguments[0], prefix: "%"));
                }
                // %xxx% LIKE queries:
                if (methodCall.Method == typeof(string).GetMethod("Contains", [typeof(string)]))
                {
                    return WherePart.Concat(Recurse(ref i, methodCall.Object), "LIKE", Recurse(ref i, methodCall.Arguments[0], prefix: "%", postfix: "%"));
                }
                // IN queries:
                if (methodCall.Method.Name == "Contains")
                {
                    Expression collection;
                    Expression property;
                    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                    {
                        collection = methodCall.Arguments[0];
                        property = methodCall.Arguments[1];
                    }
                    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                    {
                        collection = methodCall.Object;
                        property = methodCall.Arguments[0];
                    }
                    else
                    {
                        throw new JingetException("Jinget Says: Unsupported method call: " + methodCall.Method.Name);
                    }
                    var values = (IEnumerable)GetValue(collection);
                    return WherePart.Concat(Recurse(ref i, property), "IN", WherePart.IsCollection(ref i, values));
                }
                // column.ToString() OR Convert.ToString(column) queries
                if (methodCall.Method.Name == "ToString")
                {
                    //Convert.ToString(column)
                    if (methodCall.Object is null)
                    {
                        return WherePart.Cast(((MemberExpression)methodCall.Arguments[0]).Member.Name, methodCall.Method.Name);
                    }
                    //column.ToString()
                    else
                    {
                        return WherePart.Cast(((MemberExpression)methodCall.Object).Member.Name, methodCall.Method.Name);
                    }
                }

                // column.ToLower() OR column.SomeMethod().ToLower() OR SomeMethod(column).ToLower()  queries
                // column.ToUpper() OR column.SomeMethod().ToUpper() OR SomeMethod(column).ToUpper()  queries
                if (new[] { "ToLower", "ToUpper" }.Contains(methodCall.Method.Name))
                {
                    if (methodCall.Object != null)
                    {
                        //column.ToLower()
                        //column.ToUpper()
                        if (methodCall.Object.NodeType == ExpressionType.MemberAccess)
                        {
                            return WherePart.IsFunction(((MemberExpression)methodCall.Object).Member.Name, methodCall.Method.Name);
                        }
                        //column.SomeMethod().ToLower() AND SomeMethod(column).ToLower()
                        //column.SomeMethod().ToUpper() AND SomeMethod(column).ToUpper()
                        if (methodCall.Object.NodeType == ExpressionType.Call)
                        {
                            return WherePart.IsFunction(Recurse(ref i, methodCall.Object).Sql, methodCall.Method.Name);
                        }
                    }
                }
                return WherePart.IsParameter(i++, Expression.Lambda(methodCall).Compile().DynamicInvoke());
            }
            throw new JingetException("Jinget Says: Unsupported expression: " + expression.GetType().Name);
        }

        private static object GetValue(Expression member)
        {
            // source: http://stackoverflow.com/a/2616980/291955
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        private static string NodeTypeToString(ExpressionType nodeType)
        {
            return nodeType switch
            {
                ExpressionType.Add => "+",
                ExpressionType.And => "&",
                ExpressionType.AndAlso => "AND",
                ExpressionType.Divide => "/",
                ExpressionType.Equal => "=",
                ExpressionType.ExclusiveOr => "^",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.Modulo => "%",
                ExpressionType.Multiply => "*",
                ExpressionType.Negate => "-",
                ExpressionType.Not => "NOT",
                ExpressionType.NotEqual => "<>",
                ExpressionType.Or => "|",
                ExpressionType.OrElse => "OR",
                ExpressionType.Subtract => "-",
                _ => throw new JingetException($"Jinget Says: Unsupported node type: {nodeType}"),
            };
        }
    }

    public class WherePart
    {
        public string? Sql { get; set; }
        public Dictionary<string, object?> Parameters { get; set; } = [];

        public static WherePart IsSql(string sql) => new()
        {
            Parameters = [],
            Sql = sql
        };

        private static string ToSqlSyntax(string method, int len = 0)
        {
            return method.ToLower() switch
            {
                "tolower" => "LOWER(@P1)",
                "toupper" => "UPPER(@P1)",
                "tostring" => $"NVARCHAR({(len <= 0 ? "MAX" : len.ToString())})",
                _ => string.Empty,
            };
        }

        public static WherePart Cast(string column, string method) =>
            new()
            {
                Sql = $"CAST({column} AS {ToSqlSyntax(method)})"
            };

        public static WherePart IsFunction(string column, string method) =>
            new()
            {
                Sql = $"{ToSqlSyntax(method).Replace("@P1", column)}"
            };

        public static WherePart IsParameter(int count, object? value) => new()
        {
            Parameters = { { count.ToString(), value } },
            Sql = $"@{count}"
        };

        public static WherePart IsCollection(ref int countStart, IEnumerable values)
        {
            var parameters = new Dictionary<string, object?>();
            var sql = new StringBuilder("(");
            foreach (object value in values)
            {
                parameters.Add((countStart).ToString(), value);
                sql.Append($"@{countStart},");
                countStart++;
            }
            if (sql.Length == 1)
            {
                sql.Append("null,");
            }
            sql[^1] = ')';//sql[len -1]
            return new WherePart
            {
                Parameters = parameters,
                Sql = sql.ToString()
            };
        }

        public static WherePart Concat(string @operator, WherePart operand) => new()
        {
            Parameters = operand.Parameters,
            Sql = $"({@operator} {operand.Sql})"
        };

        public static WherePart Concat(WherePart left, string @operator, WherePart right)
        {
            //these operators does not need to append @
            List<string> excludedList = ["IN", "AND", "OR"];
            var rightExpr = !excludedList.Contains(@operator) && !right.Sql.StartsWith("@")
                ? "@" + right.Sql.Replace("[", "").Replace("]", "")
                : right.Sql;

            return new WherePart
            {
                Parameters = left.Parameters.Union(right.Parameters).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Sql = $"({left.Sql} {@operator} {rightExpr})"
            };
        }

        public override string? ToString() => Sql;
    }
}