using System;
using System.Reflection;

namespace Jinget.Core.ExtensionMethods.Reflection;

public static class ConstructorExtensions
{
    /// <summary>
    /// Get parameterless constructor
    /// </summary>
    public static ConstructorInfo? GetDefaultConstructor(this Type type) =>
        type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

    public static Tout? InvokeDefaultConstructor<Tout>(this Type type) where Tout : class => type.GetDefaultConstructor()?.Invoke(null) as Tout;
}
