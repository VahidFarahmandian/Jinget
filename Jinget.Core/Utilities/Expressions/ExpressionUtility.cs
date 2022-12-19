using Jinget.Core.Enumerations;
using Jinget.Core.Exceptions;
using Jinget.Core.ExtensionMethods.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Jinget.Core.Utilities.Expressions
{
    public static class ExpressionUtility
    {
        /// <summary>
        /// Transform anonymouse expression to non-anonymouse expression
        /// This method is useful wherever the class's default constructor is not accessable due to its access level
        /// but you need to use this constructor to define your expression
        /// </summary>
        internal static Expression Transform(Expression source, Type type)
        {
            if (source.Type != type && source is NewExpression newExpr && newExpr.Members.Count > 0)
            {
                return Expression.MemberInit(Expression.New(type), newExpr.Members
                    .Select(m => type.GetProperty(m.Name))
                    .Zip(newExpr.Arguments, (m, e) => Expression.Bind(m, Transform(e, m.PropertyType))));
            }
            else if (source.Type != type && source is MethodCallExpression listCall && listCall.Method.IsStatic
                && listCall.Method.DeclaringType == typeof(Enumerable) &&
                listCall.Method.Name == nameof(Enumerable.ToList))
            {
                return Transform(listCall.Arguments[0], type);
            }
            else if (source.Type != type && source is MethodCallExpression call && call.Method.IsStatic
                && call.Method.DeclaringType == typeof(Enumerable) &&
                call.Method.Name == nameof(Enumerable.Select))
            {
                var sourceEnumerable = call.Arguments[0];
                var sourceSelector = (LambdaExpression)call.Arguments[1];
                var sourceElementType = sourceSelector.Parameters[0].Type;
                var targetElementType = type.GetGenericArguments()[0];
                var targetSelector = Expression.Lambda(Transform(sourceSelector.Body, targetElementType), sourceSelector.Parameters);
                var targetMethod = call.Method.GetGenericMethodDefinition().MakeGenericMethod(sourceElementType, targetElementType);
                var result = Expression.Call(targetMethod, sourceEnumerable, targetSelector);
                if (type.IsAssignableFrom(result.Type))
                    return result;
                return Expression.Call(
                    typeof(Enumerable), nameof(Enumerable.ToList), new[] { targetElementType },
                    result);
            }
            return source;
        }

        /// <summary>
        /// Try parse an expression and return a string representation of the expression in form of 'A.B.C.etc'
        /// </summary>
        /// <exception cref="JingetException"></exception>
        internal static bool TryParseExpression(Expression expression, out string path)
        {
            path = null;
            var withoutConvert = RemoveConvert(expression);

            if (withoutConvert is ConstantExpression constant)
            {
                path = constant.Value.ToString();
            }
            else if (withoutConvert is MemberExpression memberExpression)
            {
                var thisPart = memberExpression.Member.Name;
                if (!TryParseExpression(memberExpression.Expression, out var parentPart))
                {
                    return false;
                }
                path = parentPart == null ? thisPart : parentPart + "." + thisPart;
            }
            else if (withoutConvert is MethodCallExpression callExpression)
            {
                if (callExpression.Method.Name == "Select"
                    && callExpression.Arguments.Count == 2)
                {
                    if (!TryParseExpression(callExpression.Arguments[0], out var parentPart))
                    {
                        return false;
                    }
                    if (parentPart != null)
                    {
                        if (callExpression.Arguments[1] is LambdaExpression subExpression)
                        {
                            if (!TryParseExpression(subExpression.Body, out var thisPart))
                            {
                                return false;
                            }
                            if (thisPart != null)
                            {
                                path = parentPart + "." + thisPart;
                                return true;
                            }
                        }
                    }
                }
                else if (callExpression.Method.Name == "ToString")
                {
                    return TryParseExpression(
                        callExpression.Arguments.Any() ? callExpression.Arguments.First() : callExpression.Object, out path);
                }
                else if (new[] { "ToLower", "ToUpper" }.Contains(callExpression.Method.Name))
                {
                    return TryParseExpression(callExpression.Object, out path);
                }
                else if (callExpression.Method.Name == "Where")
                {
                    throw new JingetException("Jinget Says: Filtering an Include expression is not supported", type: ExceptionType.JingetInternal, ex: new NotSupportedException());
                }
                else if (callExpression.Method.Name == "OrderBy" || callExpression.Method.Name == "OrderByDescending")
                {
                    throw new JingetException("Jinget Says: Ordering an Include expression is not supported", type: ExceptionType.JingetInternal, ex: new NotSupportedException());
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes boxing
        /// </summary>
        internal static Expression RemoveConvert(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert
                   || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }

        /// <summary>
        /// iterates throw the <paramref name="properties"/> and create a member init expression
        /// </summary>
        /// <typeparam name="T">type of expression parameter</typeparam>
        /// <param name="properties">an array of member init expression properties name</param>
        public static Expression<Func<T, T>> CreateMemberInitExpression<T>(string parameterName = "Param_0", params string[] properties)
        {
            var paramExpression = Expression.Parameter(typeof(T), parameterName);

            List<MemberAssignment> bindings = new List<MemberAssignment>();

            foreach (var property in properties)
            {
                bindings.Add(Expression.Bind(
                    member: typeof(T).GetProperty(property),
                    expression: Expression.Property(paramExpression, property)
                    ));
            }

            var memberinit = Expression.MemberInit(Expression.New(typeof(T).GetDefaultConstructor()), bindings);

            return Expression.Lambda<Func<T, T>>(memberinit, paramExpression);
        }

        public static Func<T, bool> ConstructBinaryExpression<T>(string json)
        {
            var filters = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            var type = typeof(T);
            var exprVar = Expression.Parameter(type, "x");

            //if there is no filter specified, then return a true condition
            if (filters == null || !filters.Any())
            {
                return BooleanUtility.TrueCondition<T>().Compile();
            }

            //construct queries
            List<BinaryExpression> filterExpressions = new List<BinaryExpression>();
            foreach (var filter in filters)
            {
                var property = type.GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                var propertyType = property.PropertyType;
                var exprProperty = Expression.Property(exprVar, property);

                var data = Convert.ChangeType(filter.Value, propertyType);

                var expr = Expression.Equal(exprProperty, Expression.Constant(data));
                filterExpressions.Add(expr);
            }

            //merge queries
            BinaryExpression query = filterExpressions.Count > 1 ? Expression.AndAlso(filterExpressions[0], filterExpressions[1]) : filterExpressions.First();
            for (int i = 2; i < filterExpressions.Count; i++)
            {
                query = Expression.AndAlso(query, filterExpressions[i]);
            }

            var result = Expression.Lambda<Func<T, bool>>(query, exprVar);

            return result.Compile();
        }

    }
}