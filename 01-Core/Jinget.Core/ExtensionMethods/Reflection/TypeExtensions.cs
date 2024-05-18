using Jinget.Core.Exceptions;
using Jinget.Core.ExtensionMethods.Expressions;

namespace Jinget.Core.ExtensionMethods.Reflection;

public static class TypeExtensions
{
    /// <summary>
    /// Check if given type is an anonymous type or not
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <seealso cref="https://stackoverflow.com/a/2483054/4685428"/>
    /// <seealso cref="https://stackoverflow.com/a/1650895/4685428"/>
    public static bool IsAnonymousType(this Type type)
    {
        var markedWithAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), inherit: false).Any();
        var typeName = type.Name;

        return markedWithAttribute
               && (typeName.StartsWith("<>") || type.Name.StartsWith("VB$"))
               && typeName.Contains("AnonymousType");
    }

    /// <summary>
    /// Call method dynamically at runtime. 
    /// This overload used to call the public instance method
    /// </summary>
    public static object? Call(this Type type, object? caller, string name, Type[]? parameterTypes, object?[] parameterValues, params Type[] generics)
        => type.Call(caller, name, BindingFlags.Public | BindingFlags.Instance, parameterTypes, parameterValues, generics);

    /// <summary>
    /// Call method dynamically at runtime. 
    /// This overload used to call the public static method
    /// </summary>
    public static object? Call(this Type type, string name, Type[]? parameterTypes, object?[] parameterValues, params Type[] generics)
        => type.Call(null, name, BindingFlags.Public | BindingFlags.Static, parameterTypes, parameterValues, generics);

    /// <summary>
    /// Call method dynamically at runtime. 
    /// </summary>
    /// <param name="caller">object used to call the method. if the method is static, then set this as `null`</param>
    /// <param name="name">method name</param>
    /// <param name="bindingFlags"></param>
    /// <param name="parameterTypes">parameters type used to pass to the method</param>
    /// <param name="parameterValues">parameters value used to pass to the method</param>
    /// <param name="generics">if the method is a generic method, then generic types should be specified</param>
    /// <returns>Invoke the method and return the method's return value</returns>
    public static object? Call(this Type type, object? caller, string name, BindingFlags bindingFlags, Type[]? parameterTypes, object?[] parameterValues, params Type[] generics)
    {
        MethodInfo method;
        if (generics != null)
        {
            if (parameterTypes == null)
                method = type.GetMethods(bindingFlags)
                    .Where(
                    x => x.Name == name &&
                    x.GetGenericArguments().Length == generics.Length)
                    .FirstOrDefault();
            else
                method = type.GetMethods(bindingFlags)
                    .Where(
                    x => x.Name == name &&
                    x.GetGenericArguments().Length == generics.Length &&
                    x.GetParameters().Any() &&
                    x.GetParameters().All(p => parameterTypes.ToList().Contains(p.ParameterType))
                    )
                    .FirstOrDefault();
        }
        else
        {
            if (parameterTypes == null)
                method = type.GetMethod(name, bindingFlags);
            else
                method = type.GetMethod(name, bindingFlags, parameterTypes);
        }
        if (method == null)
            return null;
        if (generics != null)
            method = method.MakeGenericMethod(generics);
        try
        {
            return method.Invoke(caller, parameterValues);
        }
        catch (Exception ex) when (ex.InnerException is JingetException)
        {
            throw new JingetException("Jinget Says: " + ex.InnerException.Message, ex.InnerException);
        }
    }

    /// <summary>
    /// Get reference-type properties of the given type.
    /// By default string properties will be ignored.
    /// </summary>
    public static List<PropertyInfo> GetReferenceTypeProperties(
        this Type type,
        BindingFlags bindingFlags,
        Expression<Func<PropertyInfo, bool>>? filter = null,
        bool includeStringTypes = false)
    {
        Expression<Func<PropertyInfo, bool>> defaultFilter = x =>
            !x.PropertyType.IsValueType && (includeStringTypes == true || x.PropertyType != typeof(string));

        if (filter != null)
            return type.GetProperties(bindingFlags)
                .Where(filter.AndAlso(defaultFilter).Compile())
                .ToList();
        else
            return type.GetProperties(bindingFlags)
            .Where(defaultFilter.Compile())
            .ToList();
    }

    /// <summary>
    /// Get reference-type properties of the given type.
    /// By default string properties will be ignored.
    /// </summary>
    public static List<PropertyInfo> GetReferenceTypeProperties(
        this Type type,
        Expression<Func<PropertyInfo, bool>>? filter = null,
        bool includeStringTypes = false)
        => type.GetReferenceTypeProperties(
            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, filter, includeStringTypes);

    /// <summary>
    /// Get the default value for the calling type
    /// </summary>
    public static object GetDefaultValue(this Type type)
    {
        // Validate parameters.
        if (type == null)
            ArgumentNullException.ThrowIfNull(type, nameof(type));

        // We want an Func<object> which returns the default.
        // Create that expression here.
        Expression<Func<object>> e = Expression.Lambda<Func<object>>(
            // Have to convert to object.
            Expression.Convert(
                // The default value, always get what the *code* tells us.
                Expression.Default(type), typeof(object)
            )
        );

        // Compile and return the value.
        return e.Compile()();
    }
}
