using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Jinget.Core.ExtensionMethods.Reflection
{
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
    }
}
