using System;
using System.ComponentModel;

namespace Jinget.Core.Enums
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get Description of a specific enum value
        /// If no Description is set on a field, then the stringfied value will be returned
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Return enum value based on the given Description. 
        /// If no Description is set on a field, then field name will be compared aginst the given description
        /// </summary>
        /// <typeparam name="TEnum">typeof enum</typeparam>
        public static TEnum GetValueFromDescription<TEnum>(string description) where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                //Is there any Description attribute set for the field?
                if (
                    string.Equals(
                        Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute ? attribute.Description : field.Name,
                        description,
                        StringComparison.CurrentCultureIgnoreCase)
                    )
                    return (TEnum)field.GetValue(null);
            }

            throw new InvalidEnumArgumentException($"Enum member with description/name '{description}', not found!");
        }
    }
}