using Jinget.Core.Enumerations;
using Jinget.Core.Exceptions;
using Jinget.Core.ExtensionMethods.Reflection;
using Jinget.Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jinget.Core.ExtensionMethods.Collections;
using System.Net.NetworkInformation;

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
                path = parentPart is null ? thisPart : $"{parentPart}.{thisPart}";
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

        public static Expression<Func<T, T>> ToExpression<T>(string property, string parameterName = "Param_0")
        {
            var paramExpression = Expression.Parameter(typeof(T), parameterName);

            var memberExpression = Expression.Constant(property);

            return Expression.Lambda<Func<T, T>>(memberExpression, paramExpression);
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

        /// <summary>
        /// Construct a boolean expression based on a json input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">a key/value structured json string/object, where keys are types property and values are their value</param>
        /// <param name="treatNullOrEmptyAsTrueCondition">When <paramref name="json"/> is null or empty, then a default condition will be returned.
        /// if this parameter's value is set to true the a default true condition will be returned otherwise a defaule false condition will be returned</param>
        /// <returns></returns>
#nullable enable
        public static Expression<Func<T, bool>> ConstructBinaryExpression<T>(object? json, bool treatNullOrEmptyAsTrueCondition = true)
#nullable disable
        {
            //if there is no json object specified, then return the default true/false condition
            if (json is null)
            {
                return treatNullOrEmptyAsTrueCondition ? BooleanUtility.TrueCondition<T>() : BooleanUtility.FalseCondition<T>();
            }
            else
            {
                return json is IList<FilterCriteria>
                    ? ConstructBinaryExpression<T>(JsonConvert.DeserializeObject<IList<FilterCriteria>>(JsonConvert.SerializeObject(json)), treatNullOrEmptyAsTrueCondition)
                    : ConstructBinaryExpression<T>(json.ToString(), treatNullOrEmptyAsTrueCondition);
            }
        }

        /// <summary>
        /// Construct a boolean expression based on a json input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">a key/value structured json string/object, where keys are types property and values are their value. 
        /// for example:{'name':'vahid','age':'34'} which translates to a dictionary where 'name' and 'age' are keys and their values are 'vahid' and '34'</param>
        /// <param name="treatNullOrEmptyAsTrueCondition">When <paramref name="json"/> is null or empty, then a default condition will be returned.
        /// if this parameters value is set to true the a default true condition will be returned otherwise a defaule false condition will be returned</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ConstructBinaryExpression<T>(string json, bool treatNullOrEmptyAsTrueCondition = true)
        {
            //if there empty json string specified, then return the default true/false condition
            if (string.IsNullOrWhiteSpace(json.ToString()))
            {
                return treatNullOrEmptyAsTrueCondition ? BooleanUtility.TrueCondition<T>() : BooleanUtility.FalseCondition<T>();
            }

            var filters = JsonConvert.DeserializeObject<IDictionary<string, string>>(json).ToFilterCriteria();
            return ConstructBinaryExpression<T>(filters, treatNullOrEmptyAsTrueCondition);

        }

        public static Expression<Func<T, bool>> ConstructBinaryExpression<T>(IList<FilterCriteria> filters, bool treatNullOrEmptyAsTrueCondition = true)
        {
            //if there is no filter specified, then return the default true/false condition
            if (filters is null || !filters.Any())
            {
                return treatNullOrEmptyAsTrueCondition ? BooleanUtility.TrueCondition<T>() : BooleanUtility.FalseCondition<T>();
            }

            var type = typeof(T);
            var exprVariable = Expression.Parameter(type, "x");

            //construct queries
            IDictionary<Expression, ConditionCombiningType> filterExpressions = new Dictionary<Expression, ConditionCombiningType>();
            foreach (var filter in filters)
            {
                var property = type.GetProperty(filter.Operand, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                var propertyType = property.PropertyType;
                var propertyExpression = Expression.Property(exprVariable, property);

                var value = Convert.ChangeType(filter.Value, propertyType);

                filterExpressions.Add(GetBinaryExpression(propertyExpression, Expression.Constant(value), filter.Operator), filter.NextConditionCombination);
            }

            return Expression.Lambda<Func<T, bool>>(MergeFilterExpressions(filterExpressions), exprVariable);
        }

        private static Expression MergeFilterExpressions(IDictionary<Expression, ConditionCombiningType> filterExpressions)
        {
            Expression JoinExpressions(Expression left, ConditionCombiningType join, Expression right)
            {
                return join switch
                {
                    ConditionCombiningType.Unknown => Expression.AndAlso(left, right),
                    ConditionCombiningType.AndAlso => Expression.AndAlso(left, right),
                    ConditionCombiningType.OrElse => Expression.OrElse(left, right),
                    _ => throw new JingetException($"Conditional join of type {filterExpressions.ElementAt(0).Value} is not supported by Jinget!")
                };
            }
            Expression query = filterExpressions.Count > 1
                ? JoinExpressions(filterExpressions.ElementAt(0).Key, filterExpressions.ElementAt(0).Value, filterExpressions.ElementAt(1).Key)
                : filterExpressions.First().Key;

            for (int i = 2; i < filterExpressions.Count; i++)
            {
                query = JoinExpressions(query, filterExpressions.ElementAt(i - 1).Value, filterExpressions.ElementAt(i).Key);
            }
            return query;
        }

        private static Expression GetBinaryExpression(MemberExpression left, ConstantExpression right, Operator @operator)
        {
            return @operator switch
            {
                Operator.Equal => Expression.Equal(left, right),
                Operator.GreaterThan => Expression.GreaterThan(left, right),
                Operator.LowerThan => Expression.LessThan(left, right),
                Operator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
                Operator.LowerThanOrEqual => Expression.LessThanOrEqual(left, right),
                Operator.NotEqual => Expression.NotEqual(left, right),
                Operator.Contains => Expression.Call(left, typeof(string).GetMethod("Contains", new[] { typeof(string) }), right),
                _ => throw new JingetException($"Operator of type {@operator} is not supported by Jinget!"),
            };
        }
    }
}