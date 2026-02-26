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

    /// <summary>
    /// extract constant values from binary expression
    /// conditions like 'a'='b' where both sides are constant will be ignored
    /// </summary>
    public static List<string> ExtractValues<T>(this Expression<Func<T, bool>>? expression, bool onlyDistincts = true)
    {
        if (expression == null)
            return [];

        var extractor = new ValueExtractor<T>();
        return extractor.Extract(expression, onlyDistincts);
    }

    public class ValueExtractor<T>
    {
        public List<string> FoundValues { get; } = new List<string>();

        public List<string> Extract(Expression<Func<T, bool>> expression, bool onlyDistincts = true)
        {
            ExtractFromExpression(expression.Body);
            return onlyDistincts ? [.. FoundValues.Distinct()] : FoundValues;
        }

        private void ExtractFromExpression(Expression expr)
        {
            if (expr == null) return;

            switch (expr)
            {
                case BinaryExpression binary:
                    ExtractFromBinary(binary);
                    break;
                case MethodCallExpression method:
                    ExtractFromMethodCall(method);
                    break;
                case UnaryExpression unary:
                    ExtractFromExpression(unary.Operand);
                    break;
                case MemberExpression member:
                    ExtractValueFromExpression(member);
                    break;
                case ConstantExpression constant:
                    ExtractValueFromExpression(constant);
                    break;
            }
        }

        private void ExtractFromBinary(BinaryExpression binary)
        {
            if (binary.NodeType == ExpressionType.OrElse || binary.NodeType == ExpressionType.Or)
            {
                // Process OR in order: left then right
                ExtractFromExpression(binary.Left);
                ExtractFromExpression(binary.Right);
            }
            else if (binary.NodeType == ExpressionType.Equal || binary.NodeType == ExpressionType.NotEqual)
            {
                // Extract left side
                var leftValue = ExtractDirectValue(binary.Left);
                if (leftValue != null)
                    AddIfNotDuplicate(leftValue);
                else
                    ExtractValueFromExpression(binary.Left); // Fallback

                // Extract right side  
                var rightValue = ExtractDirectValue(binary.Right);
                if (rightValue != null)
                    AddIfNotDuplicate(rightValue);
                else
                    ExtractValueFromExpression(binary.Right); // Fallback
            }
            else
            {
                // Other binary expressions
                ExtractValueFromExpression(binary.Left);
                ExtractValueFromExpression(binary.Right);
            }
        }

        private void ExtractFromMethodCall(MethodCallExpression method)
        {
            // Handle Contains, StartsWith, EndsWith, Equals, ToString, Convert.ToString

            // Extract from instance
            if (method.Object != null)
            {
                ExtractValueFromExpression(method.Object);
            }

            // Extract from arguments in order
            foreach (var arg in method.Arguments)
            {
                ExtractValueFromExpression(arg);
            }
        }

        private string ExtractDirectValue(Expression expr)
        {
            // Try to get string value directly without recursion
            try
            {
                switch (expr)
                {
                    case ConstantExpression constant when constant.Value is string str:
                        return str; // Direct string constant like "peter" or "Joe"

                    case ConstantExpression constant when constant.Value != null &&
                                                           constant.Type != typeof(bool):
                        return constant.Value.ToString(); // Other constants

                    case MemberExpression member:
                        var memberValue = GetMemberValue(member);
                        if (memberValue != null && !IsCompilerGenerated(memberValue.GetType()))
                            return memberValue.ToString();
                        break;

                    case MethodCallExpression method:
                        var methodValue = ExecuteMethod(method);
                        if (methodValue != null)
                            return methodValue.ToString();
                        break;

                    case UnaryExpression unary when unary.NodeType == ExpressionType.Convert:
                        return ExtractDirectValue(unary.Operand);
                }
            }
            catch
            {
                // Ignore
            }

            return null;
        }

        private void ExtractValueFromExpression(Expression expr)
        {
            if (expr == null) return;

            string value = ExtractDirectValue(expr);
            if (value != null)
            {
                AddIfNotDuplicate(value);
                return;
            }

            // If we couldn't get a direct value, recurse if needed
            switch (expr)
            {
                case MemberExpression member:
                    var memberValue = GetMemberValue(member);
                    if (memberValue != null && !IsCompilerGenerated(memberValue.GetType()))
                    {
                        AddIfNotDuplicate(memberValue.ToString());
                    }
                    break;

                case MethodCallExpression method:
                    var methodValue = ExecuteMethod(method);
                    if (methodValue != null)
                    {
                        AddIfNotDuplicate(methodValue.ToString());
                    }
                    break;

                case UnaryExpression unary when unary.NodeType == ExpressionType.Convert:
                    ExtractValueFromExpression(unary.Operand);
                    break;
            }
        }

        private object GetMemberValue(MemberExpression member)
        {
            try
            {
                object instance = null;
                if (member.Expression != null)
                {
                    if (member.Expression is ConstantExpression constant)
                    {
                        instance = constant.Value;
                    }
                    else if (member.Expression is MemberExpression innerMember)
                    {
                        instance = GetMemberValue(innerMember);
                    }
                }

                if (member.Member is FieldInfo field)
                    return field.GetValue(instance);

                if (member.Member is PropertyInfo prop)
                    return prop.GetValue(instance);
            }
            catch { }

            return null;
        }

        private object ExecuteMethod(MethodCallExpression method)
        {
            try
            {
                var lambda = Expression.Lambda<Func<object>>(
                    Expression.Convert(method, typeof(object)));
                var func = lambda.Compile();
                return func();
            }
            catch
            {
                return null;
            }
        }

        private bool IsCompilerGenerated(Type type)
        {
            return type.Name.Contains("DisplayClass") ||
                   type.Name.Contains("<>c__") ||
                   type.Name.Contains("__DisplayClass") ||
                   type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();
        }

        private void AddIfNotDuplicate(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            if (value == "True" || value == "False") return;

            if (!FoundValues.Contains(value))
            {
                FoundValues.Add(value);
            }
        }
    }

}