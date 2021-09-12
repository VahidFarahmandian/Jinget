using System;
using System.Reflection;

namespace Jinget.Core.ExtensionMethods
{
    public static class TypeExtensions
    {
        public static ConstructorInfo GetDefaultConstructor(this Type type) =>
            type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
    }
}
