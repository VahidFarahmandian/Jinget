namespace Jinget.Core.Utilities;

public class BindingBuilder<TSource>
{
    private readonly List<Expression<Func<TSource, object>>> _projections = new();
    private readonly Dictionary<string, MemberBinding> _bindings = new();

    public BindingBuilder<TSource> Add(Expression<Func<TSource, object>> projection)
    {
        _projections.Add(projection);
        return this;
    }

    public BindingBuilder<TSource> AddRange(IEnumerable<Expression<Func<TSource, object>>> projections)
    {
        _projections.AddRange(projections);
        return this;
    }

    public Expression<Func<TSource, TSource>> Build()
    {
        var parameter = Expression.Parameter(typeof(TSource), "x");

        foreach (var projection in _projections)
        {
            ProcessProjection(projection.Body, parameter);
        }

        var newExpr = Expression.New(typeof(TSource));
        var memberInit = Expression.MemberInit(newExpr, _bindings.Values);
        return Expression.Lambda<Func<TSource, TSource>>(memberInit, parameter);
    }

    private void ProcessProjection(Expression expression, ParameterExpression parameter)
    {
        switch (expression)
        {
            case NewExpression newExpr:
                ProcessNewExpression(newExpr, parameter);
                break;

            case MemberInitExpression memberInit:
                ProcessMemberInit(memberInit, parameter);
                break;

            case MethodCallExpression methodCall when methodCall.Method.Name == "Select":
                ProcessSelectCall(methodCall, parameter);
                break;

            case MemberExpression memberExpr:
                ProcessMemberExpression(memberExpr, parameter);
                break;
        }
    }

    private bool IsPropertyOfParameter(Expression expr, ParameterExpression parameter)
    {
        // Unwrap any conversions
        if (expr is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            expr = unary.Operand;
        }

        if (expr is MemberExpression member && member.Member is PropertyInfo)
        {
            // Handle the case where expression is a constant value (like in new { Trace = new { ... } })
            if (member.Expression is ConstantExpression)
            {
                return false;
            }

            // Walk up the expression tree to find the root parameter
            Expression current = member.Expression;
            while (current != null)
            {
                switch (current)
                {
                    // Handle parameter match
                    case ParameterExpression currentParam
                        when currentParam.Name == parameter.Name &&
                             currentParam.Type == parameter.Type:
                        return true;

                    // Continue up member access chain
                    case MemberExpression currentMember:
                        current = currentMember.Expression;
                        continue;

                    // Handle other cases
                    default:
                        return false;
                }
            }
        }

        return false;
    }
    private Expression GetPropertyAccessExpression(Expression expr, ParameterExpression parameter)
    {
        // Handle direct property access
        if (expr is MemberExpression member && member.Expression == parameter)
        {
            return member;
        }

        // Handle nested property access
        if (expr is MemberExpression nestedMember)
        {
            var propertyChain = new List<PropertyInfo>();
            Expression current = nestedMember;

            while (current is MemberExpression currentMember &&
                   currentMember.Member is PropertyInfo currentProp)
            {
                propertyChain.Insert(0, currentProp);
                current = currentMember.Expression;
            }

            if (current == parameter && propertyChain.Count > 0)
            {
                Expression result = parameter;
                foreach (var prop in propertyChain)
                {
                    result = Expression.Property(result, prop);
                }
                return result;
            }
        }

        return expr;
    }

    private void ProcessNewExpression(NewExpression newExpr, ParameterExpression parameter)
    {
        if (newExpr.Type.IsAnonymousType())
        {
            var properties = newExpr.Type.GetProperties();

            for (int i = 0; i < newExpr.Arguments.Count; i++)
            {
                var arg = newExpr.Arguments[i];
                var propName = properties[i].Name;
                var propInfo = parameter.Type.GetProperty(propName);

                if (propInfo == null) continue;

                if (IsPropertyOfParameter(arg, parameter))
                {
                    var propertyAccess = GetPropertyAccessExpression(arg, parameter);
                    _bindings[propName] = Expression.Bind(propInfo, propertyAccess);
                }
                else if (arg is MethodCallExpression methodCall && methodCall.Method.Name == "Select")
                {
                    ProcessSelectCall(methodCall, parameter);
                }
                else if (arg is NewExpression nestedNew && nestedNew.Type.IsAnonymousType())
                {
                    var nestedInit = BuildNestedInitialization(nestedNew, parameter);
                    _bindings[propName] = Expression.Bind(propInfo, nestedInit);
                }
            }
        }
    }

    private Expression BuildNestedInitialization(NewExpression newExpr, ParameterExpression parameter)
    {
        // Get the target property and type from the expression structure
        var (targetProperty, targetType) = GetTargetPropertyAndType(newExpr, parameter);
        if (targetProperty == null || targetType == null)
            return null;

        var bindings = new List<MemberBinding>();
        var visitor = new MemberBindingVisitor(parameter);

        // Process each argument in the anonymous type constructor
        for (int i = 0; i < newExpr.Arguments.Count; i++)
        {
            var arg = newExpr.Arguments[i];
            var propName = newExpr.Type.GetProperties()[i].Name;

            // Find the corresponding property in the target type
            var targetProp = targetType.GetProperty(propName);
            if (targetProp == null || !targetProp.CanWrite)
                continue;

            // Process the argument expression
            var processedArg = visitor.Visit(arg);
            if (processedArg is MemberExpression memberExpr)
            {
                bindings.Add(Expression.Bind(targetProp, memberExpr));
            }
        }

        if (bindings.Count == 0)
            return null;

        // Create the member initialization expression
        return Expression.MemberInit(Expression.New(targetType), bindings);
    }

    private (PropertyInfo, Type) GetTargetPropertyAndType(NewExpression newExpr, ParameterExpression parameter)
    {
        if (newExpr.Arguments.FirstOrDefault() is MemberExpression memberExpr)
        {
            var propertyChain = new List<PropertyInfo>();
            var current = memberExpr;

            // Build the property access chain
            while (current != null)
            {
                if (current.Member is PropertyInfo prop)
                    propertyChain.Insert(0, prop);

                // Check if we've reached the parameter (compare by name and type)
                if (current.Expression is ParameterExpression currentParam &&
                    currentParam.Name == parameter.Name &&
                    currentParam.Type == parameter.Type)
                {
                    if (current.Member is PropertyInfo rootProp)
                    {
                        // The target type is the type of the last property in the chain
                        Type reflectedType = parameter.Type;
                        Type currentType = parameter.Type;
                        PropertyInfo targetProp = null;

                        foreach (var property in propertyChain)
                        {
                            targetProp = currentType.GetProperty(property.Name);
                            if (targetProp == null) break;
                            currentType = targetProp.PropertyType;
                            reflectedType = targetProp.ReflectedType;
                        }

                        return (targetProp, reflectedType);
                    }
                    break;
                }

                current = current.Expression as MemberExpression;
            }
        }

        return (null, null);
    }
    private class MemberBindingVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public MemberBindingVisitor(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member is PropertyInfo prop)
            {
                var propertyChain = new List<PropertyInfo>();
                Expression current = node;

                // Walk up the property chain
                while (current is MemberExpression currentMember &&
                       currentMember.Member is PropertyInfo currentProp)
                {
                    propertyChain.Insert(0, currentProp);
                    current = currentMember.Expression;
                }

                // Rebuild from our root parameter
                Expression result = _parameter;
                foreach (var property in propertyChain)
                {
                    var targetProp = result.Type.GetProperty(property.Name);
                    if (targetProp == null) return node;

                    result = Expression.Property(result, targetProp);
                }

                return result;
            }

            return base.VisitMember(node);
        }
    }

    private Expression RebuildPropertyAccess(MemberExpression member, ParameterExpression parameter)
    {
        var propertyChain = new List<PropertyInfo>();
        Expression current = member;

        // Walk up the property access chain
        while (current is MemberExpression currentMember &&
               currentMember.Member is PropertyInfo currentProp)
        {
            propertyChain.Insert(0, currentProp);
            current = currentMember.Expression;
        }

        // Start from the parameter and rebuild the chain
        Expression result = parameter;
        foreach (var prop in propertyChain)
        {
            // Get the property with full access
            var writableProp = result.Type.GetProperty(prop.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (writableProp == null) return null;

            result = Expression.Property(result, writableProp);
        }

        return result;
    }

    private void ProcessMemberExpression(MemberExpression memberExpr, ParameterExpression parameter)
    {
        if (IsPropertyOfParameter(memberExpr, parameter) && memberExpr.Member is PropertyInfo prop)
        {
            AddPropertyBinding(prop, memberExpr);
        }
    }

    private void ProcessMemberInit(MemberInitExpression memberInit, ParameterExpression parameter)
    {
        foreach (var binding in memberInit.Bindings)
        {
            if (binding is MemberAssignment assignment)
            {
                _bindings[assignment.Member.Name] = assignment;
            }
        }
    }

    private void ProcessSelectCall(MethodCallExpression selectCall, ParameterExpression rootParameter)
    {
        if (!(selectCall.Arguments[0] is MemberExpression collectionAccess) ||
            !(collectionAccess.Member is PropertyInfo collectionProp))
            return;

        // Get the lambda expression from the Select call
        LambdaExpression lambda = selectCall.Arguments[1] switch
        {
            UnaryExpression unary when unary.Operand is LambdaExpression unaryLambda => unaryLambda,
            LambdaExpression directLambda => directLambda,
            _ => throw new InvalidOperationException("Unexpected Select argument type")
        };

        var elementType = collectionProp.PropertyType.GetGenericArguments()[0];
        var elementParam = lambda.Parameters[0];

        var elementInit = BuildElementInitialization(lambda.Body, elementParam, elementType);

        var selectMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
            .MakeGenericMethod(elementType, elementInit.Type);

        var toListMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
            .MakeGenericMethod(elementInit.Type);

        // Correct way to update the lambda
        var newLambda = Expression.Lambda(
            elementInit,
            lambda.TailCall,
            lambda.Parameters
        );

        var select = Expression.Call(
            selectMethod,
            Expression.Property(rootParameter, collectionProp),
            newLambda);

        var toList = Expression.Call(toListMethod, select);

        _bindings[collectionProp.Name] = Expression.Bind(collectionProp, toList);
    }

    private bool IsPropertyOfSource(Expression expr, ParameterExpression parameter)
    {
        // Unwrap any type conversions
        while (expr is UnaryExpression unary &&
               (unary.NodeType == ExpressionType.Convert ||
                unary.NodeType == ExpressionType.ConvertChecked))
        {
            expr = unary.Operand;
        }

        // We're only interested in property access expressions
        if (expr is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo)
        {
            return false;
        }

        // Walk up the expression tree to find the root
        Expression current = memberExpr.Expression;
        while (current != null)
        {
            // Handle parameter match (compare by name and type, not reference equality)
            if (current is ParameterExpression currentParam &&
                currentParam.Name == parameter.Name &&
                currentParam.Type == parameter.Type)
            {
                return true;
            }

            // Continue up member access chain
            if (current is MemberExpression currentMember)
            {
                current = currentMember.Expression;
                continue;
            }

            // Handle other cases (like constants or method calls)
            break;
        }

        return false;
    }
    private Expression BuildElementInitialization(Expression expression, ParameterExpression elementParam, Type elementType)
    {
        if (expression is not NewExpression newExpr)
            return expression;

        var bindings = new List<MemberBinding>();

        foreach (var (arg, member) in newExpr.Arguments.Zip(newExpr.Members, (a, m) => (a, m)))
        {
            var propName = member.Name;
            var targetProp = elementType.GetProperty(propName);
            if (targetProp == null) continue;

            if (arg is MemberExpression memberExpr && IsPropertyOfSource(memberExpr, elementParam))
            {
                // Simple property binding
                bindings.Add(Expression.Bind(targetProp, memberExpr));
            }
            else if (arg is NewExpression nestedNew && nestedNew.Type.IsAnonymousType())
            {
                // Nested anonymous type
                var nestedInit = BuildNestedInitialization(nestedNew, elementParam);
                if (nestedInit != null)
                {
                    bindings.Add(Expression.Bind(targetProp, nestedInit));
                }
            }
            else if (arg is MethodCallExpression methodCall && methodCall.Method.Name == "Select")
            {
                // Nested collection projection
                if (methodCall.Arguments[0] is MemberExpression collectionAccess &&
                    collectionAccess.Member is PropertyInfo collectionProp)
                {
                    var lambda = methodCall.Arguments[1] switch
                    {
                        UnaryExpression unary when unary.Operand is LambdaExpression unaryLambda => unaryLambda,
                        LambdaExpression directLambda => directLambda,
                        _ => null
                    };

                    if (lambda != null)
                    {
                        var nestedElementType = collectionProp.PropertyType.GetGenericArguments()[0];
                        var nestedElementParam = lambda.Parameters[0];
                        var nestedElementInit = BuildElementInitialization(lambda.Body, nestedElementParam, nestedElementType);

                        var selectMethod = typeof(Enumerable).GetMethods()
                            .First(m => m.Name == "Select" &&
                            m.GetParameters().Length == 2 &&
                            m.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2)
                            .MakeGenericMethod(nestedElementType, nestedElementInit.Type);

                        var toListMethod = typeof(Enumerable).GetMethod("ToList")?
                            .MakeGenericMethod(nestedElementInit.Type);

                        if (selectMethod != null && toListMethod != null)
                        {
                            var select = Expression.Call(
                                selectMethod,
                                Expression.Property(elementParam, collectionProp),
                                Expression.Lambda(nestedElementInit, lambda.Parameters));

                            var toList = Expression.Call(toListMethod, select);
                            bindings.Add(Expression.Bind(targetProp, toList));
                        }
                    }
                }
            }
        }

        return Expression.MemberInit(Expression.New(elementType), bindings);
    }
    private void AddPropertyBinding(PropertyInfo property, MemberExpression memberExpr)
    {
        if (!_bindings.ContainsKey(property.Name))
        {
            _bindings[property.Name] = Expression.Bind(property, memberExpr);
        }
    }
}

public static class BindingDefinition
{
    public static BindingBuilder<TSource> CreateBuilder<TSource>()
    {
        return new BindingBuilder<TSource>();
    }

    public static Expression<Func<TSource, TSource>> Compile<TSource>(
        params Expression<Func<TSource, TSource>>[] projections)
    {
        var builder = CreateBuilder<TSource>();
        foreach (var projection in projections)
        {
            // Need to convert Expression<Func<TSource, TSource>> to Expression<Func<TSource, object>>
            var converted = Expression.Lambda<Func<TSource, object>>(
                projection.Body,
                projection.Parameters);
            builder.Add(converted);
        }
        return builder.Build();
    }
}