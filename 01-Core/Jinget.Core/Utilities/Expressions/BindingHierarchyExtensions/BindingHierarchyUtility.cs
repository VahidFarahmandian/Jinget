namespace Jinget.Core.Utilities.Expressions.BindingHierarchyExtensions;

internal static class BindingHierarchyUtility
{
    internal static Expression<Func<TSource, TSource>> CreateBindingExpression<TSource>(List<BindingHierarchy> bindings)
    {
        var parameter = Expression.Parameter(typeof(TSource), "x");
        var bindingsList = new List<MemberBinding>();

        foreach (var binding in bindings)
        {
            var memberBinding = CreateMemberBinding<TSource>(parameter, binding);
            if (memberBinding != null)
            {
                bindingsList.Add(memberBinding);
            }
        }

        var newExpression = Expression.New(typeof(TSource));
        var memberInitExpression = Expression.MemberInit(newExpression, bindingsList);

        return Expression.Lambda<Func<TSource, TSource>>(memberInitExpression, parameter);
    }

    private static MemberBinding CreateMemberBinding<TSource>(Expression parameter, BindingHierarchy binding)
    {
        // Get the target property on the root type (TestClass)
        var targetProperty = typeof(TSource).GetProperty(binding.PropertyName);
        if (targetProperty == null)
        {
            // This property doesn't exist directly on TestClass - it must be a nested property
            return HandleNestedProperty<TSource>(parameter, binding);
        }

        // Simple property case (no parent)
        if (binding.ParentProperty == null)
        {
            var sourceAccess = Expression.Property(parameter, targetProperty);
            return Expression.Bind(targetProperty, sourceAccess);
        }

        // Property exists on root but has parent - this is invalid
        throw new ArgumentException($"Property {binding.PropertyName} exists on root type but has parent binding");
    }

    private static MemberBinding HandleNestedProperty<TSource>(Expression parameter, BindingHierarchy binding)
    {
        // Build the full property path
        var (sourceAccess, sourcePropertyName, targetType) = BuildPropertyPath<TSource>(parameter, binding);

        // Get the root property (the one directly on TSource)
        var rootProperty = typeof(TSource).GetProperty(sourcePropertyName);
        if (rootProperty == null)
        {
            // If this is a collection projection, we need to wrap it in the appropriate object initialization
            if (sourceAccess.Type.IsCollectionType() && binding.ParentProperty != null)
            {
                rootProperty = typeof(TSource).GetProperty(GetRootProperty(binding));
                // Build the object initialization hierarchy
                var memberInit = BuildObjectInitialization(binding, sourceAccess);
                return Expression.Bind(rootProperty, memberInit);
            }
            else
                throw new ArgumentException($"Property {GetRootProperty(binding)} not found on type {typeof(TSource).Name}");
        }


        return Expression.Bind(rootProperty, sourceAccess);
    }

    private static Expression BuildObjectInitialization(BindingHierarchy binding, Expression collectionProjection)
    {
        // Start from the innermost binding
        var current = binding;
        while (current.ParentProperty?.ParentProperty != null)
        {
            current = current.ParentProperty;
        }

        // Create the innermost object initialization
        var innerType = current.InitiatorPropertyType;
        var newExpr = Expression.New(innerType);
        var property = innerType.GetProperty(current.PropertyName);
        var memberInit = Expression.MemberInit(newExpr,
            Expression.Bind(property, collectionProjection));

        return memberInit;
    }

    private static string GetRootProperty(BindingHierarchy binding)
    {
        // Walk up the hierarchy to find the root property
        while (binding.ParentProperty != null)
        {
            binding = binding.ParentProperty;
        }
        return binding.PropertyName;
    }

    private static (Expression, string, Type) BuildPropertyPath<TSource>(Expression parameter, BindingHierarchy binding)
    {
        // Convert the binding hierarchy into a stack
        var bindingStack = new Stack<BindingHierarchy>();
        var current = binding;
        while (current != null)
        {
            bindingStack.Push(current);
            current = current.ParentProperty;
        }

        // Initialize tracking variables
        Expression currentExpression = parameter;
        Type currentSourceType = typeof(TSource);
        PropertyInfo sourceProperty = currentSourceType.GetProperty(bindingStack.Peek().PropertyName);

        // This will store our property access chain
        var propertyChain = new Stack<PropertyInfo>();

        while (bindingStack.Count > 0)
        {
            var currentBinding = bindingStack.Pop();

            // Get source property
            sourceProperty = currentSourceType.GetProperty(currentBinding.PropertyName);
            if (sourceProperty == null)
            {
                throw new ArgumentException($"Property {currentBinding.PropertyName} not found on type {currentSourceType.Name}");
            }

            propertyChain.Push(sourceProperty);

            // Handle collection properties
            if (sourceProperty.PropertyType.IsCollectionType() && bindingStack.Count > 0)
            {
                var sourceElementType = sourceProperty.PropertyType.GetGenericArguments()[0];
                var lambdaParam = Expression.Parameter(sourceElementType, "x");

                // Process the next binding level recursively
                var remainingBindings = new Stack<BindingHierarchy>(bindingStack.Reverse());
                var (innerProjection, _, _, lastBinding) = BuildCollectionProjection(
                    lambdaParam,
                    remainingBindings,
                    sourceElementType);

                // Create object initialization
                var newExpr = Expression.New(sourceElementType);
                var memberInit = Expression.MemberInit(newExpr,
                    Expression.Bind(sourceElementType.GetProperty(lastBinding.PropertyName), innerProjection));

                // Create lambda
                var lambda = Expression.Lambda(memberInit, lambdaParam);

                // Create Select call
                var selectMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(sourceElementType, sourceElementType);

                currentExpression = Expression.Call(
                    selectMethod,
                    Expression.Property(currentExpression, sourceProperty),
                    lambda);

                // Add ToList()
                var toListMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
                    .MakeGenericMethod(sourceElementType);

                currentExpression = Expression.Call(toListMethod, currentExpression);
                currentSourceType = sourceElementType;
                bindingStack.Clear(); // We've processed all remaining bindings
            }
            else
            {
                // Handle regular properties
                currentExpression = Expression.Property(currentExpression, sourceProperty);
                currentSourceType = sourceProperty.PropertyType;
            }
        }

        // Now build the expression for non-collection properties
        if (!propertyChain.Peek().PropertyType.IsCollectionType())
        {
            var propertyAccess = BuildNestedPropertyAccess(parameter, propertyChain);
            return (propertyAccess, GetRootProperty(binding), currentSourceType);
        }
        else
            return (currentExpression, sourceProperty?.Name, currentSourceType);

    }

    private static Expression BuildNestedPropertyAccess(Expression parameter, Stack<PropertyInfo> propertyChain)
    {
        // We need to process from inner to outer
        var reversedChain = new Stack<PropertyInfo>(propertyChain);

        // Start with the parameter as our base expression
        Expression currentExpression = parameter;
        MemberInitExpression memberInit = null;

        // First, build the property access chain (x.InnerSingularProperty.Parent_1.Sub)
        foreach (var source in reversedChain)
        {
            currentExpression = Expression.Property(currentExpression, source);
        }

        // Now build the initialization from inner to outer
        while (propertyChain.Count > 1)
        {
            var target = propertyChain.Pop();

            if (memberInit == null)
            {
                // Innermost level - create new object with the full property access
                var newExpr = Expression.New(target.DeclaringType);
                memberInit = Expression.MemberInit(newExpr,
                    Expression.Bind(target, currentExpression));
            }
            else
            {
                // Outer levels - wrap the previous initialization
                var newExpr = Expression.New(target.DeclaringType);
                memberInit = Expression.MemberInit(newExpr,
                    Expression.Bind(target, memberInit));
            }
        }

        return memberInit;
    }

    private static (Expression, Type, BindingHierarchy, BindingHierarchy) BuildCollectionProjection(
        Expression elementParameter,
        Stack<BindingHierarchy> bindingStack,
        Type currentSourceType)
    {
        if (bindingStack.Count == 0)
        {
            throw new InvalidOperationException("No more bindings to process");
        }

        var currentBinding = bindingStack.Pop();

        // Get source property
        var sourceProperty = currentSourceType.GetProperty(currentBinding.PropertyName);
        if (sourceProperty == null)
        {
            throw new ArgumentException($"Property {currentBinding.PropertyName} not found on type {currentSourceType.Name}");
        }

        // Base case - no more bindings
        if (bindingStack.Count == 0)
        {
            return (Expression.Property(elementParameter, sourceProperty), sourceProperty.PropertyType/*targetProperty.PropertyType*/, currentBinding, currentBinding);
        }

        // Handle nested collections
        if (sourceProperty.PropertyType.IsCollectionType())
        {
            var sourceElementType = sourceProperty.PropertyType.GetGenericArguments()[0];
            var nestedLambdaParam = Expression.Parameter(sourceElementType, "y");

            // Process next binding level
            var (innerProjection, innerType, CollectionBinding, _) = BuildCollectionProjection(
                nestedLambdaParam,
                bindingStack,
                sourceElementType
                /*targetElementType*/);

            // Create object initialization
            var newExpr = Expression.New(sourceElementType);
            var memberInitExpr = Expression.MemberInit(newExpr,
                Expression.Bind(sourceElementType.GetProperty(CollectionBinding.PropertyName), innerProjection));

            // Create lambda
            var lambda = Expression.Lambda(memberInitExpr, nestedLambdaParam);

            // Create Select call
            var selectMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .MakeGenericMethod(sourceElementType, sourceElementType /*targetElementType*/);

            var selectCall = Expression.Call(
                selectMethod,
                Expression.Property(elementParameter, sourceProperty),
                lambda);

            // Add ToList()
            var toListMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
                .MakeGenericMethod(sourceElementType);

            return (Expression.Call(toListMethod, selectCall), typeof(List<>).MakeGenericType(sourceElementType), CollectionBinding, currentBinding);
        }

        // Handle regular nested properties
        var propertyAccess = Expression.Property(elementParameter, sourceProperty);
        var (innerResult, resultType, RegularBinding, _) = BuildCollectionProjection(
            propertyAccess,
            bindingStack,
            sourceProperty.PropertyType);

        var propName = "";
        if (bindingStack.Count == 0)
            propName = innerResult.ToString().Split('.', StringSplitOptions.RemoveEmptyEntries).Last();
        else
            propName = bindingStack.Peek().PropertyName;

        var newObject = Expression.New(sourceProperty.PropertyType);
        var memberInit = Expression.MemberInit(newObject,
            Expression.Bind(sourceProperty.PropertyType.GetProperty(propName), innerResult));

        return (memberInit, sourceProperty.PropertyType, RegularBinding, currentBinding);
    }

    private static (Expression, Type) BuildPropertyPathForCollectionElement(Expression elementParameter, BindingHierarchy binding)
    {
        Expression currentExpression = elementParameter;
        Type currentType = elementParameter.Type;
        var currentBinding = binding;

        // Handle nested collections
        if (currentType.IsCollectionType() && currentBinding.ParentProperty != null)
        {
            var elementType = currentType.GetGenericArguments()[0];
            var lambdaParam = Expression.Parameter(elementType, "p");

            // Recursively handle nested collection
            var innerBinding = currentBinding.ParentProperty;
            var (innerExpression, innerType) = BuildPropertyPathForCollectionElement(lambdaParam, innerBinding);

            // Create object initialization
            var newExpr = Expression.New(elementType);
            var memberInit = Expression.MemberInit(newExpr,
                Expression.Bind(elementType.GetProperty(innerBinding.PropertyName), innerExpression));

            // Create lambda
            var lambda = Expression.Lambda(memberInit, lambdaParam);

            // Create Select call
            var selectMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .MakeGenericMethod(elementType, elementType);

            var selectCall = Expression.Call(selectMethod,
                Expression.Property(currentExpression, currentType.GetProperty(currentBinding.PropertyName)),
                lambda);

            // Create ToList call
            var toListMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
                .MakeGenericMethod(elementType);

            return (Expression.Call(toListMethod, selectCall), typeof(List<>).MakeGenericType(elementType));
        }

        // Handle regular property access
        var property = currentType.GetProperty(binding.PropertyName);
        if (property == null)
        {
            throw new ArgumentException($"Property {binding.PropertyName} not found on type {currentType.Name}");
        }

        return (Expression.Property(currentExpression, property), property.PropertyType);
    }

    private static MemberBinding CreateMemberBindingCore(Type currentType, Expression sourceAccess, BindingHierarchy binding)
    {
        if (sourceAccess.Type.IsCollectionType())
        {
            return HandleCollectionPropertyCore(currentType, sourceAccess, binding);
        }
        else
        {
            var property = currentType.GetProperty(binding.PropertyName);
            if (property == null)
            {
                throw new ArgumentException($"Property {binding.PropertyName} not found on type {currentType.Name}");
            }

            if (binding.ParentProperty != null)
            {
                var nestedSourceAccess = Expression.Property(sourceAccess, property);
                var nestedBinding = CreateMemberBindingCore(property.PropertyType, nestedSourceAccess, binding.ParentProperty);

                var newExpr = Expression.New(property.PropertyType);
                var memberInit = Expression.MemberInit(newExpr, nestedBinding);
                return Expression.Bind(property, memberInit);
            }
            else
            {
                return Expression.Bind(property, Expression.Property(sourceAccess, property));
            }
        }
    }

    private static MemberBinding HandleCollectionPropertyCore(Type currentType, Expression sourceAccess, BindingHierarchy binding)
    {
        var elementType = sourceAccess.Type.GetGenericArguments()[0];
        var lambdaParam = Expression.Parameter(elementType, "i");

        if (binding.ParentProperty != null)
        {
            var nestedProperty = elementType.GetProperty(binding.PropertyName);
            if (nestedProperty == null)
            {
                throw new ArgumentException($"Property {binding.PropertyName} not found on type {elementType.Name}");
            }

            var nestedSourceAccess = Expression.Property(lambdaParam, nestedProperty);
            var nestedBinding = CreateMemberBindingCore(nestedProperty.PropertyType, nestedSourceAccess, binding.ParentProperty);

            var newExpr = Expression.New(elementType);
            var memberInit = Expression.MemberInit(newExpr, nestedBinding);

            var lambda = Expression.Lambda(memberInit, lambdaParam);

            var selectMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .MakeGenericMethod(elementType, elementType);

            var selectCall = Expression.Call(selectMethod, sourceAccess, lambda);

            var toListMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
                .MakeGenericMethod(elementType);

            var toListCall = Expression.Call(toListMethod, selectCall);

            var property = currentType.GetProperty(binding.PropertyName);
            return Expression.Bind(property, toListCall);
        }
        else
        {
            var property = elementType.GetProperty(binding.PropertyName);
            var newExpr = Expression.New(elementType);
            var memberInit = Expression.MemberInit(newExpr,
                Expression.Bind(property, Expression.Property(lambdaParam, property)));

            var lambda = Expression.Lambda(memberInit, lambdaParam);

            var selectMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .MakeGenericMethod(elementType, elementType);

            var selectCall = Expression.Call(selectMethod, sourceAccess, lambda);

            var toListMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
                .MakeGenericMethod(elementType);

            var toListCall = Expression.Call(toListMethod, selectCall);

            var targetProperty = currentType.GetProperty(binding.PropertyName);
            return Expression.Bind(targetProperty, toListCall);
        }
    }

    private static (Expression, Type) BuildPropertyPathForElement(Expression elementParameter, Stack<BindingHierarchy> hierarchy)
    {
        Expression currentExpression = elementParameter;
        Type currentType = elementParameter.Type;

        while (hierarchy.Count > 0)
        {
            var currentBinding = hierarchy.Pop();
            var property = currentType.GetProperty(currentBinding.PropertyName);
            if (property == null)
            {
                throw new ArgumentException($"Property {currentBinding.PropertyName} not found on type {currentType.Name}");
            }

            currentExpression = Expression.Property(currentExpression, property);
            currentType = property.PropertyType;

            // If we encounter another collection in the middle of the path
            if (currentType.IsCollectionType() && hierarchy.Count > 0)
            {
                var elementType = currentType.GetGenericArguments()[0];
                var lambdaParam = Expression.Parameter(elementType, "j");

                var (innerExpression, innerType) = BuildPropertyPathForElement(lambdaParam, hierarchy);

                var lambda = Expression.Lambda(innerExpression, lambdaParam);

                var selectMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(elementType, innerType);

                currentExpression = Expression.Call(selectMethod, currentExpression, lambda);
                currentType = innerType;

                hierarchy.Clear();
            }
        }

        return (currentExpression, currentType);
    }
}