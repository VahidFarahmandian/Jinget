namespace Jinget.Core.Utilities.Expressions;

public static class ExpressionUtility
{
    /// <summary>
    /// Transform anonymous expression to non-anonymous expression
    /// This method is useful wherever the class's default constructor is not accessible due to its access level
    /// , but you need to use this constructor to define your expression
    /// </summary>
    internal static Expression Transform(Expression source, Type type)
    {
        if (source.Type != type && source is NewExpression newExpr && newExpr.Members?.Count > 0)
        {
            var bindings = new List<MemberBinding>();
            foreach (var member in newExpr.Members)
            {
                var targetProperty = type.GetProperty(member.Name);
                if (targetProperty != null) // Skip if property doesn't exist in target type
                {
                    var argument = newExpr.Arguments[newExpr.Members.IndexOf(member)];
                    bindings.Add(Expression.Bind(targetProperty, Transform(argument, targetProperty.PropertyType)));
                }
                else
                    throw new Exception($"Jinget Says: Property {member.Name} does not exists in {type.Name}");
            }
            return Expression.MemberInit(Expression.New(type), bindings);
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
                typeof(Enumerable), nameof(Enumerable.ToList), [targetElementType],
                result);
        }
        return source;
    }

    /// <summary>
    /// Try to parse an expression and return a string representation of the expression in the form of 'A.B.C.etc.'
    /// </summary>
    /// <exception cref="JingetException"></exception>
    internal static bool TryParseExpression(Expression? expression, out string? path)
    {
        path = null;
        if (expression == null)
            return true;
        var withoutConvert = RemoveConvert(expression);

        if (withoutConvert is ConstantExpression constant)
        {
            path = constant.Value?.ToString();
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
    /// Remove boxing
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
    /// example return: x=>"id"
    /// </summary>
    public static Expression<Func<T, T>> ToExpression<T>(string property, string parameterName = "Param_0")
    {

        var paramExpression = Expression.Parameter(typeof(T), parameterName);

        var memberExpression = Expression.Constant(property);

        return Expression.Lambda<Func<T, T>>(memberExpression, paramExpression);
    }

    public static Expression<Func<Tin, Tout>> ToExpression<Tin, Tout>(string property, string parameterName = "Param_0")
    {

        var paramExpression = Expression.Parameter(typeof(Tin), parameterName);

        var memberExpression = Expression.Constant(property);

        return Expression.Lambda<Func<Tin, Tout>>(memberExpression, paramExpression);
    }

    /// <summary>
    /// example return: x=>x.id
    /// </summary>
    public static Func<T, object> ToSelectorExpression<T>(string property, string parameterName = "Param_0")
    {
        var item = Expression.Parameter(typeof(T), parameterName);
        var body = property.Split('.').Aggregate((Expression)item, Expression.PropertyOrField);
        var lambda = Expression.Lambda(body, item);

        return Expression.Lambda<Func<T, object>>(lambda, item).Compile();
    }

    /// <summary>
    /// iterates throw the <paramref name="properties"/> and create a member init expression
    /// </summary>
    /// <typeparam name="T">type of expression parameter</typeparam>
    /// <param name="properties">an array of member init expression properties name</param>
    public static Expression<Func<T, T>> CreateMemberInitExpression<T>(
        string parameterName = "Param_0",
        params string[] properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        if (properties.Length == 0)
            throw new ArgumentException(
                "At least one property path is required.",
                nameof(properties));

        var paths = properties
            .Select(path =>
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException(
                        "Property paths cannot be empty.",
                        nameof(properties));

                var segments = path.Split('.');

                if (segments.Any(string.IsNullOrWhiteSpace))
                    throw new ArgumentException(
                        $"Invalid property path: '{path}'.",
                        nameof(properties));

                return segments;
            })
            .ToArray();

        var parameter = Expression.Parameter(typeof(T), parameterName);

        var body = BuildMemberInit(
            typeof(T),
            parameter,
            paths);

        return Expression.Lambda<Func<T, T>>(body, parameter);
    }

    public static Expression<Func<Tin, Tout>> CreateMemberInitExpression<Tin, Tout>(
            string parameterName = "Param_0",
            params string[] properties)
    {
        Expression<Func<Tin, Tin>> expression =
            CreateMemberInitExpression<Tin>(parameterName, properties);

        if (!typeof(Tout).IsAssignableFrom(expression.Body.Type))
        {
            throw new InvalidOperationException(
                $"Cannot use expression body of type '{expression.Body.Type}' " +
                $"as return type '{typeof(Tout)}' without a conversion.");
        }

        return Expression.Lambda<Func<Tin, Tout>>(
            expression.Body,
            expression.Parameters);
    }

    private static Expression BuildMemberInit(
        Type targetType,
        Expression source,
        string[][] paths)
    {
        var bindings = new List<MemberBinding>();

        var groups = paths.GroupBy(
            path => path[0],
            StringComparer.OrdinalIgnoreCase);

        foreach (var group in groups)
        {
            var property = GetProperty(targetType, group.Key);

            var childPaths = group
                .Select(path => path.Skip(1).ToArray())
                .ToArray();

            if (childPaths.Any(path => path.Length == 0))
            {
                if (childPaths.Length > 1)
                {
                    throw new ArgumentException(
                        $"Property '{property.Name}' is selected more than once.");
                }

                if (group.Count() > 1)
                {
                    throw new ArgumentException(
                        $"Property '{property.Name}' conflicts with a nested path.");
                }

                bindings.Add(Expression.Bind(
                    property,
                    Expression.Property(source, property)));

                continue;
            }

            var sourceProperty = Expression.Property(source, property);

            Expression nestedExpression;

            if (TryGetEnumerableElementType(
                property.PropertyType,
                out var elementType))
            {
                nestedExpression = BuildCollectionProjection(
                    property.PropertyType,
                    sourceProperty,
                    elementType,
                    childPaths);
            }
            else
            {
                nestedExpression = BuildMemberInit(
                    property.PropertyType,
                    sourceProperty,
                    childPaths);
            }

            bindings.Add(Expression.Bind(
                property,
                nestedExpression));
        }

        var constructor = targetType.GetConstructor(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance,
            binder: null,
            Type.EmptyTypes,
            modifiers: null);

        if (constructor is null)
        {
            throw new InvalidOperationException(
                $"Type '{targetType.Name}' has no parameterless constructor.");
        }

        return Expression.MemberInit(
            Expression.New(constructor),
            bindings);
    }

    private static Expression BuildCollectionProjection(
        Type collectionType,
        Expression sourceCollection,
        Type elementType,
        string[][] paths)
    {
        var parameter = Expression.Parameter(elementType, "item");

        var projectedElement = BuildMemberInit(
            elementType,
            parameter,
            paths);

        var selectorType = typeof(Func<,>).MakeGenericType(
            elementType,
            elementType);

        var selector = Expression.Lambda(
            selectorType,
            projectedElement,
            parameter);

        var selectMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method =>
                method.Name == nameof(Enumerable.Select) &&
                method.IsGenericMethodDefinition &&
                method.GetParameters().Length == 2 &&
                method.GetParameters()[1].ParameterType
                    .GetGenericArguments().Length == 2);

        var selectCall = Expression.Call(
            selectMethod.MakeGenericMethod(elementType, elementType),
            sourceCollection,
            selector);

        var enumerableType = typeof(IEnumerable<>)
            .MakeGenericType(elementType);

        if (collectionType.IsArray)
        {
            var toArrayMethod = typeof(Enumerable)
                .GetMethod(nameof(Enumerable.ToArray))!
                .MakeGenericMethod(elementType);

            return Expression.Call(toArrayMethod, selectCall);
        }

        var listType = typeof(List<>).MakeGenericType(elementType);

        var toListMethod = typeof(Enumerable)
            .GetMethod(nameof(Enumerable.ToList))!
            .MakeGenericMethod(elementType);

        var listCall = Expression.Call(toListMethod, selectCall);

        if (collectionType.IsAssignableFrom(listType))
            return listCall;

        var enumerableConstructor = collectionType.GetConstructor(
            new[] { enumerableType });

        if (enumerableConstructor is not null)
        {
            return Expression.New(
                enumerableConstructor,
                selectCall);
        }

        throw new NotSupportedException(
            $"Collection type '{collectionType.Name}' cannot be constructed " +
            $"from a projected sequence of '{elementType.Name}'.");
    }

    private static PropertyInfo GetProperty(
        Type type,
        string propertyName)
    {
        var property = type.GetProperty(
            propertyName,
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.IgnoreCase);

        if (property is null)
        {
            throw new ArgumentException(
                $"Property '{propertyName}' was not found on type '{type.Name}'.");
        }

        if (property.GetIndexParameters().Length > 0)
        {
            throw new NotSupportedException(
                $"Indexer property '{property.Name}' is not supported.");
        }

        if (property.SetMethod is null)
        {
            throw new ArgumentException(
                $"Property '{property.Name}' must have a setter.");
        }

        return property;
    }

    private static bool TryGetEnumerableElementType(
        Type type,
        out Type elementType)
    {
        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        var enumerableType = type.IsGenericType &&
                             type.GetGenericTypeDefinition() ==
                             typeof(IEnumerable<>)
            ? type
            : type.GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() ==
                    typeof(IEnumerable<>));

        if (enumerableType is not null)
        {
            elementType = enumerableType.GetGenericArguments()[0];
            return true;
        }

        elementType = null!;
        return false;
    }

    /// <summary>
    /// Construct a boolean expression based on a JSON input
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">a key/value structured JSON string/object, where keys are types property and values are their value</param>
    /// <param name="treatNullOrEmptyAsTrueCondition">When <paramref name="json"/> is null or empty, then a default condition will be returned.
    /// if this parameter's value is set to true, the default true condition will be returned, otherwise a default false condition will be returned</param>
    /// <returns></returns>
#nullable enable
    public static Expression<Func<T, bool>> ConstructBinaryExpression<T>(object? json, bool treatNullOrEmptyAsTrueCondition = true)
#nullable disable
    {
        //if there is no JSON object specified, then return the default true/false condition
        if (json is null)
        {
            return treatNullOrEmptyAsTrueCondition ? BooleanUtility.TrueCondition<T>() : BooleanUtility.FalseCondition<T>();
        }
        else
        {
            return json is IList<FilterCriteria>
                ? ConstructBinaryExpression<T>(json.Serialize().Deserialize<IList<FilterCriteria>>(), treatNullOrEmptyAsTrueCondition)
                : ConstructBinaryExpression<T>(json.ToString(), treatNullOrEmptyAsTrueCondition);
        }
    }

    /// <summary>
    /// Construct a boolean expression based on a JSON input
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">a key/value structured JSON string/object, where keys are types property and values are their value. 
    /// for example:{'name':'vahid','age':'34'} which translates to a dictionary where 'name' and 'age' are keys and their values are 'vahid' and '34'</param>
    /// <param name="treatNullOrEmptyAsTrueCondition">When <paramref name="json"/> is null or empty, then a default condition will be returned.
    /// if this parameter value is set to true, the default true condition will be returned, otherwise a default false condition will be returned</param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> ConstructBinaryExpression<T>(string json, bool treatNullOrEmptyAsTrueCondition = true)
    {
        //if there empty JSON string specified, then return the default true/false condition
        if (string.IsNullOrWhiteSpace(json.ToString()))
        {
            return treatNullOrEmptyAsTrueCondition ? BooleanUtility.TrueCondition<T>() : BooleanUtility.FalseCondition<T>();
        }

        var filters = json.Deserialize<IDictionary<string, string>>(strictPropertyMatching: false).ToFilterCriteria();
        return ConstructBinaryExpression<T>(filters, treatNullOrEmptyAsTrueCondition);

    }

    public static Expression<Func<T, bool>> ConstructBinaryExpression<T>(
    IList<FilterCriteria> filters,
    bool treatNullOrEmptyAsTrueCondition = true)
    {
        if (filters is null || !filters.Any())
        {
            return treatNullOrEmptyAsTrueCondition
                ? BooleanUtility.TrueCondition<T>()
                : BooleanUtility.FalseCondition<T>();
        }

        var type = typeof(T);
        var exprVariable = Expression.Parameter(type, "x");

        IDictionary<Expression, ConditionJoinType> filterExpressions = new Dictionary<Expression, ConditionJoinType>();

        foreach (var filter in filters)
        {
            var propertyExpression = GetPropertyExpression(
                exprVariable,
                filter.Operand);

            Expression rightExpression;

            switch (filter.Operator)
            {
                case Operator.IsNull:
                case Operator.IsNotNull:
                    rightExpression = Expression.Constant(
                        null,
                        propertyExpression.Type);
                    break;

                case Operator.Between:
                    {
                        if (filter.Value is not System.Collections.IEnumerable values
                            || filter.Value is string)
                        {
                            throw new JingetException(
                                "The Between operator requires exactly two values.");
                        }

                        var bounds = values.Cast<object>().ToArray();

                        if (bounds.Length != 2)
                        {
                            throw new JingetException(
                                "The Between operator requires exactly two values.");
                        }

                        var valueType =
                            Nullable.GetUnderlyingType(propertyExpression.Type)
                            ?? propertyExpression.Type;

                        var lower = Convert.ChangeType(bounds[0], valueType);
                        var upper = Convert.ChangeType(bounds[1], valueType);

                        rightExpression = Expression.NewArrayInit(
                            propertyExpression.Type,
                            Expression.Convert(
                                Expression.Constant(lower, valueType),
                                propertyExpression.Type),
                            Expression.Convert(
                                Expression.Constant(upper, valueType),
                                propertyExpression.Type));

                        break;
                    }

                case Operator.In:
                case Operator.NotIn:
                    {
                        if (filter.Value is not System.Collections.IEnumerable values
                            || filter.Value is string)
                        {
                            throw new JingetException(
                                "The In/NotIn operator requires a collection of values.");
                        }

                        var propertyType = propertyExpression.Type;
                        var underlyingType =
                            Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                        var convertedValues = values.Cast<object>()
                            .Select(value =>
                                value is null
                                    ? null
                                    : Convert.ChangeType(value, underlyingType))
                            .ToArray();

                        var typedArray = Array.CreateInstance(
                            propertyType,
                            convertedValues.Length);

                        for (var i = 0; i < convertedValues.Length; i++)
                        {
                            typedArray.SetValue(convertedValues[i], i);
                        }

                        rightExpression = Expression.Constant(
                            typedArray,
                            propertyType.MakeArrayType());

                        break;
                    }

                default:
                    {
                        var valueType =
                            Nullable.GetUnderlyingType(propertyExpression.Type)
                            ?? propertyExpression.Type;

                        var value = Convert.ChangeType(filter.Value, valueType);

                        rightExpression = Expression.Constant(
                            value,
                            propertyExpression.Type);

                        break;
                    }
            }

            filterExpressions.Add(
                GetBinaryExpression(
                    propertyExpression,
                    rightExpression,
                    filter.Operator),
                filter.NextConditionCombination);
        }

        return Expression.Lambda<Func<T, bool>>(
            MergeFilterExpressions(filterExpressions),
            exprVariable);
    }

    private static Expression GetPropertyExpression(
    Expression expression,
    string propertyPath)
    {
        foreach (var propertyName in propertyPath.Split('.'))
        {
            var property = expression.Type.GetProperty(
                propertyName,
                BindingFlags.IgnoreCase |
                BindingFlags.Instance |
                BindingFlags.Public);

            if (property is null)
            {
                throw new ArgumentException(
                    $"Property '{propertyName}' was not found on type '{expression.Type.Name}'.",
                    nameof(propertyPath));
            }

            expression = Expression.Property(expression, property);
        }

        return expression;
    }

    /// <summary>
    /// create expression to search among all string properties
    /// </summary>
    /// <param name="preserveCase">if set to false then call to ToLower method will be attached to all properties </param>
    /// <param name="conditionJoinType">defines whether to join conditions using OrElse or AndAlso</param>
    public static Expression<Func<TModelType, bool>> CreateSearchAllColumnsExpression<TModelType>(
        string searchTerm,
        string paramName = "x",
        bool preserveCase = false,
        ConditionJoinType conditionJoinType = ConditionJoinType.OrElse)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            // Return an expression that always evaluates to true
            return BooleanUtility.TrueCondition<TModelType>();
        }

        if (!preserveCase)
            searchTerm = searchTerm.ToLowerInvariant();

        // Get all string properties of the entity
        var stringProperties = typeof(TModelType).GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        // Create a predicate for each string property
        var predicates = stringProperties.Select(p =>
        {
            // Create an expression for accessing the property value
            var parameter = Expression.Parameter(typeof(TModelType), paramName);
            var propertyAccess = Expression.Property(parameter, p);
            var methodCall = Expression.Call(propertyAccess, "ToLower", null);
            var constant = Expression.Constant(searchTerm);
            var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
            var body = Expression.Call(preserveCase ? propertyAccess : methodCall, containsMethod, constant);
            return Expression.Lambda<Func<TModelType, bool>>(body, parameter);
        }).ToList();

        // Combine all predicates using OR logic
        Expression<Func<TModelType, bool>> combinedPredicate = null;
        foreach (var predicate in predicates)
        {
            if (combinedPredicate == null)
            {
                combinedPredicate = predicate;
            }
            else
            {
                if (conditionJoinType == ConditionJoinType.OrElse)
                    combinedPredicate = combinedPredicate.OrElse(predicate, paramName);
                else
                    combinedPredicate = combinedPredicate.AndAlso(predicate, paramName);
            }
        }

        return combinedPredicate;
    }

    private static Expression MergeFilterExpressions(IDictionary<Expression, ConditionJoinType> filterExpressions)
    {
        Expression JoinExpressions(Expression left, ConditionJoinType join, Expression right)
        {
            return join switch
            {
                ConditionJoinType.Unknown => Expression.AndAlso(left, right),
                ConditionJoinType.AndAlso => Expression.AndAlso(left, right),
                ConditionJoinType.OrElse => Expression.OrElse(left, right),
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

    private static Expression GetBinaryExpression(
    Expression left,
    Expression right,
    Operator @operator) => @operator switch
    {
        // Equality
        Operator.Equal => Expression.Equal(left, right),
        Operator.NotEqual => Expression.NotEqual(left, right),

        // Comparison
        Operator.GreaterThan => Expression.GreaterThan(left, right),
        Operator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
        Operator.LowerThan => Expression.LessThan(left, right),
        Operator.LowerThanOrEqual => Expression.LessThanOrEqual(left, right),

        // String operations
        Operator.Contains => CreateStringCall(left, right, nameof(string.Contains)),
        Operator.NotContains => Expression.Not(
            CreateStringCall(left, right, nameof(string.Contains))),

        Operator.StartsWith => CreateStringCall(left, right, nameof(string.StartsWith)),
        Operator.DoesNotStartWith => Expression.Not(
            CreateStringCall(left, right, nameof(string.StartsWith))),

        Operator.EndsWith => CreateStringCall(left, right, nameof(string.EndsWith)),
        Operator.DoesNotEndWith => Expression.Not(
            CreateStringCall(left, right, nameof(string.EndsWith))),

        // Null checks
        Operator.IsNull => Expression.Equal(
            left,
            Expression.Constant(null, left.Type)),

        Operator.IsNotNull => Expression.NotEqual(
            left,
            Expression.Constant(null, left.Type)),

        // Collection membership
        Operator.In => CreateMembershipExpression(left, right),
        Operator.NotIn => Expression.Not(CreateMembershipExpression(left, right)),

        // Range: right must represent a two-element collection/array
        Operator.Between => CreateBetweenExpression(left, right),

        _ => throw new JingetException(
            $"Operator of type {@operator} is not supported by Jinget!")
    };

    private static Expression CreateStringCall(
    Expression left,
    Expression right,
    string methodName)
    {
        if (left.Type != typeof(string) || right.Type != typeof(string))
        {
            throw new JingetException(
                $"Operator '{methodName}' requires string operands.");
        }

        var overload = typeof(string).GetMethod(
            methodName,
            [typeof(string), typeof(StringComparison)]);

        if (overload is null)
        {
            throw new JingetException(
                $"String method '{methodName}' is not supported.");
        }

        return Expression.Call(
            left,
            overload,
            right,
            Expression.Constant(StringComparison.OrdinalIgnoreCase));
    }

    private static Expression CreateMembershipExpression(
        Expression left,
        Expression right)
    {
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(left.Type);

        if (!enumerableType.IsAssignableFrom(right.Type))
        {
            throw new JingetException(
                $"The In/NotIn operator requires a collection of '{left.Type.Name}' values.");
        }

        var containsMethod = typeof(Enumerable)
            .GetMethods()
            .Single(method =>
                method.Name == nameof(Enumerable.Contains) &&
                method.GetParameters().Length == 2)
            .MakeGenericMethod(left.Type);

        return Expression.Call(containsMethod, right, left);
    }

    private static Expression CreateBetweenExpression(
        Expression left,
        Expression right)
    {
        if (right is not NewArrayExpression array ||
            array.Expressions.Count != 2)
        {
            throw new JingetException(
                "The Between operator requires an array containing exactly two values.");
        }

        return Expression.AndAlso(
            Expression.GreaterThanOrEqual(left, array.Expressions[0]),
            Expression.LessThanOrEqual(left, array.Expressions[1]));
    }

    public static Expression<Func<T, bool>> CreateEqualCondition<T, TValueType>(string propertyName, object value, string parameterName = "Param_0")
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), parameterName);

        Type valueType = typeof(TValueType);

        var rightExpression = Expression.Constant(value, valueType.IsNullable() ? Nullable.GetUnderlyingType(valueType) : valueType);

        MemberExpression propertyAccess = GetMemberAccessExpression(parameter, propertyName, typeof(T));
        if (valueType.IsNullable())
        {
            MemberExpression propertyValueAccess = GetMemberAccessExpression(parameter, $"{propertyName}.Value", typeof(T));
            MemberExpression hasValueAccess = GetMemberAccessExpression(parameter, $"{propertyName}.HasValue", typeof(T));
            return Expression.Lambda<Func<T, bool>>(
                Expression.Condition(hasValueAccess, Expression.Equal(propertyValueAccess, rightExpression), Expression.Constant(false)), parameter);
        }

        return Expression.Lambda<Func<T, bool>>(Expression.Equal(propertyAccess, rightExpression), parameter);
    }

    private static MemberExpression GetMemberAccessExpression(Expression expression, string members, Type type)
    {
        List<string> memberList = [.. members.Split(["."], StringSplitOptions.RemoveEmptyEntries)];
        string firstMember = memberList.First();
        MemberExpression memberExpression = CreateMemberAccessExpression(type, expression, firstMember);

        foreach (var item in memberList.Skip(1))
        {
            memberExpression = CreateMemberAccessExpression(((PropertyInfo)memberExpression.Member).PropertyType, memberExpression, item);
        }
        return memberExpression;
    }

    private static MemberExpression CreateMemberAccessExpression(Type type, Expression expression, string member)
    {
        var membersInfo = type.GetMember(member);
        if (membersInfo.Any())
        {
            MemberInfo memberInfo = membersInfo.FirstOrDefault();
            if (memberInfo != null)
                return Expression.MakeMemberAccess(expression, memberInfo);

            throw new JingetException($"Jinget Says: {type.Name} does not contain {member} member.", 1000);
        }

        throw new JingetException($"Jinget Says: {type.Name} does not contain {member} member.", 1000);
    }
}