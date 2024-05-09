using System;

namespace Jinget.Core.ExtensionMethods.Generics;

public static class GenericTypeExtensions
{
    /// <summary>
    /// Check if a type is a child of another type(including generic and non-generic types)
    /// This method is very likely to IsSubclassOf method but also supports generic types too
    /// </summary>
    public static bool IsSubclassOfRawGeneric(this Type derivedType, Type parentType)
    {
        while (derivedType != null && derivedType != typeof(object))

        {
            var currentType = derivedType.IsGenericType ? derivedType.GetGenericTypeDefinition() : derivedType;
            if (parentType == currentType)
            {
                return true;
            }
            derivedType = derivedType.BaseType;
        }
        return false;
    }
}
