namespace Jinget.Core.Utilities;

public class BindingBuilder<TSource>
{
    private readonly List<Expression<Func<TSource, object>>> _projections = [];
    private readonly Dictionary<string, MemberBinding> _memberBindings = [];

    #region Public API

    /// <summary>
    /// افزودن یک projection جدید به بیلدر
    /// </summary>
    public BindingBuilder<TSource> Add(Expression<Func<TSource, object>> projection)
    {
        _projections.Add(projection);
        return this;
    }

    /// <summary>
    /// افزودن مجموعه‌ای از projections به بیلدر
    /// </summary>
    public BindingBuilder<TSource> AddRange(IEnumerable<Expression<Func<TSource, object>>> projections)
    {
        _projections.AddRange(projections);
        return this;
    }

    /// <summary>
    /// ساخت expression نهایی بایندینگ
    /// </summary>
    public Expression<Func<TSource, TSource>> Build()
    {
        var sourceParameter = Expression.Parameter(typeof(TSource), "x");
        _memberBindings.Clear();

        foreach (var projection in _projections)
        {
            ProcessProjection(projection.Body, sourceParameter);
        }

        var newExpression = Expression.New(typeof(TSource));
        var memberInit = Expression.MemberInit(newExpression, _memberBindings.Values);
        return Expression.Lambda<Func<TSource, TSource>>(memberInit, sourceParameter);
    }

    #endregion

    #region Core Processing Pipeline

    /// <summary>
    /// پردازش اصلی projection بر اساس نوع expression
    /// </summary>
    private void ProcessProjection(Expression expression, ParameterExpression sourceParameter)
    {
        switch (expression)
        {
            case NewExpression newExpr:
                ProcessAnonymousType(newExpr, sourceParameter);
                break;

            case MemberInitExpression memberInit:
                ProcessMemberInitialization(memberInit, sourceParameter);
                break;

            case MethodCallExpression methodCall when methodCall.Method.Name == "Select":
                ProcessSelectAtRootLevel(methodCall, sourceParameter);
                break;

            case MemberExpression memberExpr:
                ProcessSimpleMemberAccess(memberExpr, sourceParameter);
                break;
        }
    }

    /// <summary>
    /// پردازش anonymous type و استخراج اطلاعات بایندینگ از اون
    /// </summary>
    private void ProcessAnonymousType(NewExpression anonymousTypeExpr, ParameterExpression sourceParameter)
    {
        if (!anonymousTypeExpr.Type.IsAnonymousType())
            return;

        var anonymousProperties = anonymousTypeExpr.Type.GetProperties();

        for (int i = 0; i < anonymousTypeExpr.Arguments.Count; i++)
        {
            var argument = anonymousTypeExpr.Arguments[i];
            var propertyName = anonymousProperties[i].Name;
            var targetProperty = typeof(TSource).GetProperty(propertyName);

            if (targetProperty == null) continue;

            ProcessAnonymousTypeArgument(argument, sourceParameter, targetProperty, propertyName);
        }
    }

    /// <summary>
    /// پردازش هر آرگومان داخل anonymous type
    /// </summary>
    private void ProcessAnonymousTypeArgument(Expression argument, ParameterExpression sourceParameter,
        PropertyInfo targetProperty, string propertyName)
    {
        // حالت ۱: دسترسی مستقیم به پراپرتی (مثل x.Name)
        if (IsPropertyAccessFromSource(argument, sourceParameter))
        {
            var propertyAccess = BuildPropertyAccessChain(argument, sourceParameter);
            if (propertyAccess != null)
            {
                _memberBindings[propertyName] = Expression.Bind(targetProperty, propertyAccess);
            }
        }
        // حالت ۲: anonymous type تودرتو (مثل Customer = new { ... })
        else if (argument is NewExpression nestedAnonymous && nestedAnonymous.Type.IsAnonymousType())
        {
            ProcessNestedAnonymousType(nestedAnonymous, sourceParameter, targetProperty);
        }
        // حالت ۳: Select در سطح پراپرتی (مثل Files = x.Files.Select(...))
        else if (argument is MethodCallExpression methodCall && methodCall.Method.Name == "Select")
        {
            ProcessSelectForProperty(methodCall, sourceParameter, targetProperty);
        }
    }

    #endregion

    #region Property Access Processing

    /// <summary>
    /// بررسی می‌کنه که expression به یک پراپرتی از پارامتر سورس دسترسی داره یا نه
    /// </summary>
    private bool IsPropertyAccessFromSource(Expression expr, ParameterExpression sourceParameter)
    {
        if (expr is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            expr = unary.Operand;
        }

        return expr is MemberExpression memberExpr &&
               ExtractPropertyChain(memberExpr, sourceParameter)?.Count > 0;
    }

    /// <summary>
    /// استخراج زنجیره پراپرتی‌ها از یک MemberExpression
    /// </summary>
    private List<PropertyInfo> ExtractPropertyChain(MemberExpression memberExpr, ParameterExpression expectedRoot)
    {
        var chain = new List<PropertyInfo>();
        Expression current = memberExpr;

        // پیمایش به عقب در درخت expression برای یافتن زنجیره پراپرتی‌ها
        while (current is MemberExpression currentMember && currentMember.Member is PropertyInfo prop)
        {
            chain.Insert(0, prop);
            current = currentMember.Expression;
        }

        // تأیید که زنجیره به پارامتر مورد انتظار ختم می‌شه
        // باید با Type مقایسه کنیم نه reference equality
        // چون پارامترها instance های مختلفی می‌توانند باشند
        return current is ParameterExpression paramExpr &&
               paramExpr.Type == expectedRoot.Type ? chain : null;
    }

    /// <summary>
    /// ساخت expression دسترسی به پراپرتی از روی زنجیره
    /// </summary>
    private Expression BuildPropertyAccessChain(Expression expr, ParameterExpression sourceParameter)
    {
        if (expr is not MemberExpression memberExpr)
            return null;

        var propertyChain = ExtractPropertyChain(memberExpr, sourceParameter);
        if (propertyChain == null)
            return null;

        Expression result = sourceParameter;
        foreach (var prop in propertyChain)
        {
            var targetProp = result.Type.GetProperty(prop.Name);
            if (targetProp == null) return null;
            result = Expression.Property(result, targetProp);
        }

        return result;
    }

    #endregion

    #region Nested Anonymous Type Processing

    /// <summary>
    /// پردازش anonymous type های تودرتو
    /// </summary>
    private void ProcessNestedAnonymousType(NewExpression nestedAnonymous, ParameterExpression sourceParameter,
        PropertyInfo parentProperty)
    {
        var nestedType = parentProperty.PropertyType;
        var nestedBindings = new List<MemberBinding>();

        var anonymousProperties = nestedAnonymous.Type.GetProperties();

        for (int i = 0; i < nestedAnonymous.Arguments.Count; i++)
        {
            var argument = nestedAnonymous.Arguments[i];
            var propertyName = anonymousProperties[i].Name;

            var nestedProperty = nestedType.GetProperty(propertyName);
            if (nestedProperty == null) continue;

            // پردازش سطوح عمیق‌تر anonymous type
            if (argument is NewExpression deeperAnonymous && deeperAnonymous.Type.IsAnonymousType())
            {
                var deeperInit = BuildDeepAnonymousTypeInitialization(deeperAnonymous, sourceParameter, nestedProperty);
                if (deeperInit != null)
                {
                    nestedBindings.Add(Expression.Bind(nestedProperty, deeperInit));
                }
            }
            // پردازش دسترسی مستقیم به پراپرتی
            else if (IsPropertyAccessFromSource(argument, sourceParameter))
            {
                var propertyAccess = BuildPropertyAccessChain(argument, sourceParameter);
                if (propertyAccess != null)
                {
                    nestedBindings.Add(Expression.Bind(nestedProperty, propertyAccess));
                }
            }
        }

        if (nestedBindings.Count > 0)
        {
            var nestedInit = Expression.MemberInit(Expression.New(nestedType), nestedBindings);
            _memberBindings[parentProperty.Name] = Expression.Bind(parentProperty, nestedInit);
        }
    }

    /// <summary>
    /// ساخت initialization برای anonymous type های عمیق
    /// </summary>
    private Expression BuildDeepAnonymousTypeInitialization(NewExpression anonymousExpr,
        ParameterExpression sourceParameter, PropertyInfo targetProperty)
    {
        var targetType = targetProperty.PropertyType;
        var bindings = new List<MemberBinding>();

        var anonymousProperties = anonymousExpr.Type.GetProperties();

        for (int i = 0; i < anonymousExpr.Arguments.Count; i++)
        {
            var argument = anonymousExpr.Arguments[i];
            var propertyName = anonymousProperties[i].Name;

            var nestedProp = targetType.GetProperty(propertyName);
            if (nestedProp == null) continue;

            if (argument is MemberExpression memberExpr)
            {
                var propertyChain = ExtractPropertyChain(memberExpr, sourceParameter);
                if (propertyChain != null)
                {
                    var propertyAccess = BuildPropertyAccessFromChain(sourceParameter, propertyChain);
                    bindings.Add(Expression.Bind(nestedProp, propertyAccess));
                }
            }
        }

        return bindings.Count > 0
            ? Expression.MemberInit(Expression.New(targetType), bindings)
            : null;
    }

    /// <summary>
    /// ساخت دسترسی به پراپرتی از روی زنجیره (ورژن جایگزین)
    /// </summary>
    private Expression BuildPropertyAccessFromChain(ParameterExpression parameter, List<PropertyInfo> propertyChain)
    {
        Expression result = parameter;
        foreach (var prop in propertyChain)
        {
            var targetProp = result.Type.GetProperty(prop.Name);
            if (targetProp == null) return null;
            result = Expression.Property(result, targetProp);
        }
        return result;
    }

    #endregion

    #region Select Method Processing

    /// <summary>
    /// پردازش Select در سطح root (بدون association با پراپرتی خاص)
    /// </summary>
    private void ProcessSelectAtRootLevel(MethodCallExpression selectCall, ParameterExpression sourceParameter)
    {
        if (selectCall.Arguments[0] is not MemberExpression collectionAccess ||
            collectionAccess.Member is not PropertyInfo collectionProperty)
            return;

        var lambda = ExtractLambdaFromSelect(selectCall.Arguments[1]);
        if (lambda == null) return;

        // حالت خاص: اگر body خودش یک متد call باشه
        if (lambda.Body is MethodCallExpression methodCall)
        {
            HandleSpecialMethodCallCase(methodCall, sourceParameter, collectionProperty, lambda);
            return;
        }

        // حالت عادی: پردازش projection داخل Select
        ProcessStandardSelectProjection(selectCall, sourceParameter, collectionProperty, null);
    }

    /// <summary>
    /// پردازش Select برای یک پراپرتی خاص
    /// </summary>
    private void ProcessSelectForProperty(MethodCallExpression selectCall, ParameterExpression sourceParameter,
        PropertyInfo targetProperty)
    {
        if (selectCall.Arguments[0] is not MemberExpression collectionAccess ||
            collectionAccess.Member is not PropertyInfo collectionProperty)
            return;

        var lambda = ExtractLambdaFromSelect(selectCall.Arguments[1]);
        if (lambda == null) return;

        // حالت خاص: اگر body خودش یک متد call باشه
        if (lambda.Body is MethodCallExpression methodCall)
        {
            HandleSpecialMethodCallCase(methodCall, sourceParameter, collectionProperty, lambda, targetProperty);
            return;
        }

        // حالت عادی
        ProcessStandardSelectProjection(selectCall, sourceParameter, collectionProperty, targetProperty);
    }

    /// <summary>
    /// استخراج lambda expression از آرگومان Select
    /// </summary>
    private LambdaExpression ExtractLambdaFromSelect(Expression selectArgument)
    {
        return selectArgument switch
        {
            UnaryExpression unary when unary.Operand is LambdaExpression lambda => lambda,
            LambdaExpression lambda => lambda,
            _ => null
        };
    }

    /// <summary>
    /// پردازش حالت خاص که body lambda خودش یک متد call باشه
    /// </summary>
    private void HandleSpecialMethodCallCase(MethodCallExpression methodCall, ParameterExpression sourceParameter,
        PropertyInfo collectionProperty, LambdaExpression lambda, PropertyInfo targetProperty = null)
    {
        var projectionExpression = methodCall.Method.Invoke(methodCall.Object, null) as LambdaExpression;
        if (projectionExpression == null) return;

        // جایگزینی پارامتر lambda با پارامتر جدید
        var elementParam = Expression.Parameter(
            projectionExpression.Parameters[0].Type,
            projectionExpression.Parameters[0].Name ?? "x");
        var bodyReplacer = new ParameterReplacer(projectionExpression.Parameters[0], elementParam);
        var newBody = bodyReplacer.Visit(projectionExpression.Body);

        var elementType = collectionProperty.PropertyType.GetGenericArguments()[0];
        var selectResult = BuildSelectExpression(
            Expression.Property(sourceParameter, collectionProperty),
            elementParam,
            newBody,
            elementType);

        var propertyToBind = targetProperty ?? collectionProperty;
        _memberBindings[propertyToBind.Name] = Expression.Bind(propertyToBind, selectResult);
    }

    /// <summary>
    /// پردازش projection استاندارد داخل Select
    /// </summary>
    private void ProcessStandardSelectProjection(MethodCallExpression selectCall, ParameterExpression sourceParameter,
        PropertyInfo collectionProperty, PropertyInfo targetProperty)
    {
        var lambda = ExtractLambdaFromSelect(selectCall.Arguments[1]);
        if (lambda == null) return;

        var sourceElementType = collectionProperty.PropertyType.GetGenericArguments()[0];
        var targetElementType = targetProperty?.PropertyType.GetGenericArguments()[0] ?? sourceElementType;

        var elementInitialization = BuildElementInitialization(
            lambda.Body,
            lambda.Parameters[0],
            sourceElementType,
            targetElementType);

        var selectResult = BuildSelectExpression(
            Expression.Property(sourceParameter, collectionProperty),
            lambda.Parameters[0],
            elementInitialization,
            sourceElementType);

        var propertyToBind = targetProperty ?? collectionProperty;
        _memberBindings[propertyToBind.Name] = Expression.Bind(propertyToBind, selectResult);
    }

    /// <summary>
    /// ساخت expression کامل Select().ToList()
    /// </summary>
    private Expression BuildSelectExpression(Expression collection, ParameterExpression elementParam,
        Expression projectionBody, Type sourceElementType)
    {
        var selectMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
            .MakeGenericMethod(sourceElementType, projectionBody.Type);

        var toListMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
            .MakeGenericMethod(projectionBody.Type);

        var select = Expression.Call(selectMethod, collection, Expression.Lambda(projectionBody, elementParam));
        return Expression.Call(toListMethod, select);
    }

    #endregion

    #region Element Initialization (برای Select های تودرتو)

    /// <summary>
    /// ساخت initialization برای المنت‌های داخل collection
    /// </summary>
    private Expression BuildElementInitialization(Expression expression, ParameterExpression elementParam,
        Type sourceElementType, Type targetElementType)
    {
        // اگر expression یک anonymous type نباشه، همون رو برمی‌گردونیم
        if (expression is not NewExpression newExpr || !newExpr.Type.IsAnonymousType())
            return expression;

        var bindings = new List<MemberBinding>();
        var anonymousProperties = newExpr.Type.GetProperties();

        for (int i = 0; i < newExpr.Arguments.Count; i++)
        {
            var argument = newExpr.Arguments[i];
            var propertyName = anonymousProperties[i].Name;

            var targetProperty = targetElementType.GetProperty(propertyName);
            if (targetProperty == null) continue;

            ProcessElementProperty(argument, elementParam, targetProperty, bindings);
        }

        return Expression.MemberInit(Expression.New(targetElementType), bindings);
    }

    /// <summary>
    /// پردازش هر پراپرتی داخل المنت collection
    /// </summary>
    private void ProcessElementProperty(Expression argument, ParameterExpression elementParam,
        PropertyInfo targetProperty, List<MemberBinding> bindings)
    {
        // حالت ۱: دسترسی مستقیم به پراپرتی المنت
        if (argument is MemberExpression memberExpr && IsPropertyAccessFromElement(memberExpr, elementParam))
        {
            bindings.Add(Expression.Bind(targetProperty, memberExpr));
        }
        // حالت ۲: anonymous type تودرتو داخل المنت
        else if (argument is NewExpression nestedAnonymous && nestedAnonymous.Type.IsAnonymousType())
        {
            var nestedInit = BuildNestedElementInitialization(nestedAnonymous, elementParam, targetProperty);
            if (nestedInit != null)
            {
                bindings.Add(Expression.Bind(targetProperty, nestedInit));
            }
        }
        // حالت ۳: Select تودرتو داخل المنت
        else if (argument is MethodCallExpression methodCall && methodCall.Method.Name == "Select")
        {
            var selectInit = BuildNestedSelectExpression(methodCall, elementParam, targetProperty);
            if (selectInit != null)
            {
                bindings.Add(Expression.Bind(targetProperty, selectInit));
            }
        }
    }

    /// <summary>
    /// بررسی می‌کنه که expression به پراپرتی‌های المنت دسترسی داره
    /// </summary>
    private bool IsPropertyAccessFromElement(Expression expr, ParameterExpression elementParam)
    {
        while (expr is UnaryExpression unary &&
               (unary.NodeType == ExpressionType.Convert ||
                unary.NodeType == ExpressionType.ConvertChecked))
        {
            expr = unary.Operand;
        }

        if (expr is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo)
            return false;

        Expression current = memberExpr.Expression;
        while (current != null)
        {
            if (current == elementParam)
                return true;

            current = current is MemberExpression currentMember ? currentMember.Expression : null;
        }

        return false;
    }

    /// <summary>
    /// ساخت initialization برای anonymous type های تودرتو داخل المنت
    /// </summary>
    private Expression BuildNestedElementInitialization(NewExpression anonymousExpr,
        ParameterExpression elementParam, PropertyInfo targetProperty)
    {
        var nestedType = targetProperty.PropertyType;
        var bindings = new List<MemberBinding>();

        var anonymousProperties = anonymousExpr.Type.GetProperties();

        for (int i = 0; i < anonymousExpr.Arguments.Count; i++)
        {
            var argument = anonymousExpr.Arguments[i];
            var propertyName = anonymousProperties[i].Name;

            var nestedProp = nestedType.GetProperty(propertyName);
            if (nestedProp == null) continue;

            if (argument is MemberExpression memberExpr)
            {
                var propertyChain = ExtractPropertyChainFromElement(memberExpr, elementParam);
                if (propertyChain != null)
                {
                    Expression result = elementParam;
                    foreach (var prop in propertyChain)
                    {
                        result = Expression.Property(result, prop);
                    }
                    bindings.Add(Expression.Bind(nestedProp, result));
                }
            }
            // پشتیبانی از Select های تودرتو
            else if (argument is MethodCallExpression methodCall && methodCall.Method.Name == "Select")
            {
                var selectInit = BuildNestedSelectExpression(methodCall, elementParam, nestedProp);
                if (selectInit != null)
                {
                    bindings.Add(Expression.Bind(nestedProp, selectInit));
                }
            }
        }

        return bindings.Count > 0 ? Expression.MemberInit(Expression.New(nestedType), bindings) : null;
    }

    /// <summary>
    /// استخراج زنجیره پراپرتی‌ها از المنت
    /// </summary>
    private List<PropertyInfo> ExtractPropertyChainFromElement(MemberExpression memberExpr,
        ParameterExpression elementParam)
    {
        var chain = new List<PropertyInfo>();
        Expression current = memberExpr;

        while (current is MemberExpression currentMember && currentMember.Member is PropertyInfo prop)
        {
            chain.Insert(0, prop);
            current = currentMember.Expression;
        }

        return current == elementParam ? chain : null;
    }

    /// <summary>
    /// ساخت expression برای Select های تودرتو
    /// </summary>
    private Expression BuildNestedSelectExpression(MethodCallExpression selectCall,
        ParameterExpression elementParam, PropertyInfo targetProperty)
    {
        if (selectCall.Arguments[0] is not MemberExpression collectionAccess ||
            collectionAccess.Member is not PropertyInfo collectionProp)
            return null;

        var lambda = ExtractLambdaFromSelect(selectCall.Arguments[1]);
        if (lambda == null) return null;

        var sourceElementType = collectionProp.PropertyType.GetGenericArguments()[0];
        var targetElementType = targetProperty.PropertyType.GetGenericArguments()[0];

        Expression innerProjection;

        // اگر projection یک anonymous type باشه
        if (lambda.Body is NewExpression newExpr && newExpr.Type.IsAnonymousType())
        {
            innerProjection = BuildElementInitialization(newExpr, lambda.Parameters[0],
                sourceElementType, targetElementType);
        }
        else
        {
            innerProjection = lambda.Body;
        }

        return BuildSelectExpression(
            Expression.Property(elementParam, collectionProp),
            lambda.Parameters[0],
            innerProjection,
            sourceElementType);
    }

    #endregion

    #region سایر متدهای کمکی

    /// <summary>
    /// پردازش member initialization expression
    /// </summary>
    private void ProcessMemberInitialization(MemberInitExpression memberInit, ParameterExpression sourceParameter)
    {
        foreach (var binding in memberInit.Bindings)
        {
            if (binding is MemberAssignment assignment)
            {
                _memberBindings[assignment.Member.Name] = assignment;
            }
        }
    }

    /// <summary>
    /// پردازش دسترسی ساده به member
    /// </summary>
    private void ProcessSimpleMemberAccess(MemberExpression memberExpr, ParameterExpression sourceParameter)
    {
        if (IsPropertyAccessFromSource(memberExpr, sourceParameter) &&
            memberExpr.Member is PropertyInfo property)
        {
            AddPropertyBinding(property, memberExpr);
        }
    }

    /// <summary>
    /// افزودن بایندینگ به دیکشنری (با جلوگیری از duplicate)
    /// </summary>
    private void AddPropertyBinding(PropertyInfo property, MemberExpression memberExpr)
    {
        if (!_memberBindings.ContainsKey(property.Name))
        {
            _memberBindings[property.Name] = Expression.Bind(property, memberExpr);
        }
    }

    #endregion

    #region Visitor Classes

    /// <summary>
    /// visitor برای جایگزینی پارامترها در expression
    /// </summary>
    private class ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            // Compare by type, not by reference equality
            // This handles cases where the same logical parameter has different instances
            return node.Type == oldParameter.Type &&
                   node.Name == oldParameter.Name ? newParameter : base.VisitParameter(node);
        }
    }

    #endregion
}

/// <summary>
/// کلاس کمکی برای ایجاد و کامپایل expression های بایندینگ
/// </summary>
public static class BindingDefinition
{
    /// <summary>
    /// ایجاد یک builder جدید
    /// </summary>
    public static BindingBuilder<TSource> CreateBuilder<TSource>()
    {
        return new BindingBuilder<TSource>();
    }

    /// <summary>
    /// کامپایل projection ها به expression نهایی
    /// </summary>
    public static Expression<Func<TSource, TSource>> Compile<TSource>(
        params Expression<Func<TSource, TSource>>[] projections)
    {
        var builder = CreateBuilder<TSource>();
        foreach (var projection in projections)
        {
            var converted = Expression.Lambda<Func<TSource, object>>(
                projection.Body,
                projection.Parameters);
            builder.Add(converted);
        }
        return builder.Build();
    }

    /// <summary>
    /// کامپایل projection ها به expression نهایی (ورژن مستقیم)
    /// </summary>
    public static Expression<Func<TSource, TSource>> Compile<TSource>(
        params Expression<Func<TSource, object>>[] projections)
    {
        var builder = CreateBuilder<TSource>();
        foreach (var projection in projections)
        {
            builder.Add(projection);
        }
        return builder.Build();
    }
}