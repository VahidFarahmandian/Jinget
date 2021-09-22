using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jinget.Core.ExtensionMethods
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Get parameterless constructor
        /// </summary>
        public static ConstructorInfo GetDefaultConstructor(this Type type) =>
            type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

        public static Tout InvokeDefaultConstructor<Tout>(this Type type) where Tout : class => type.GetDefaultConstructor().Invoke(null) as Tout;

        /// <summary>
        /// Check if given type is a nullable type or not
        /// </summary>
        public static bool IsNullable(this Type type) => type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

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
    }
}
