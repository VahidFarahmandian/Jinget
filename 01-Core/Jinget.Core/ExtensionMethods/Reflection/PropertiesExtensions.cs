using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jinget.Core.ExtensionMethods.Reflection;

public static class PropertiesExtensions
{
    /// <summary>
    /// Check if given type is a nullable type or not
    /// </summary>
    public static bool IsNullable(this Type type) => type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// Get all writable primitive types
    /// </summary>
    public static Dictionary<string, PropertyInfo> GetWritableProperties(this Type t)
    {
        var properties = from prop in GetDeclaredProperties(t)
                         where prop.CanWrite
                         where IsSimpleType(prop.PropertyType)
                         select prop;
        return properties.ToDictionary(x => x.Name);
    }

    private static IEnumerable<PropertyInfo> GetDeclaredProperties(Type t) =>
#if NET45
        return t.GetRuntimeProperties();
#else
        t.GetTypeInfo().DeclaredProperties;
#endif

    private static bool IsSimpleType(Type t)
    {
        while (true)
        {
            if (IsPrimitive(t))
            {
                return true;
            }
            if (t == typeof(decimal))
            {
                return true;
            }
            if (t == typeof(string))
            {
                return true;
            }
            if (t == typeof(Guid))
            {
                return true;
            }
            if (t == typeof(DateTime))
            {
                return true;
            }
            if (t.BaseType == typeof(Enum))
            {
                return true;
            }
            if (t == typeof(byte[]))
            {
                return true;
            }
            t = Nullable.GetUnderlyingType(t);
            if (t is null)
            {
                break;
            }
        }
        return false;
    }

    private static bool IsPrimitive(Type t) =>
#if NET45
        return t.IsPrimitive;
#else
        t.GetTypeInfo().IsPrimitive;
#endif
}