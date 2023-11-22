using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Jinget.Core.ExtensionMethods.Enums
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get Name property of Display attribute for a specific enum value
        /// If no Name is set on a field, then the stringfied value will be returned
        /// </summary>
        public static string GetDisplayName(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi is null)
                return string.Empty;
            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            return attributes.Length > 0 ? attributes[0].Name : value.ToString();
        }

        /// <summary>
        /// Get Description of a specific enum value
        /// If no Description is set on a field, then the stringfied value will be returned
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi is null)
                return string.Empty;
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Return enum value based on the given Description. 
        /// If no Description is set on a field, then field name will be compared aginst the given description
        /// </summary>
        /// <typeparam name="TEnum">typeof enum</typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
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
                {
                    var value = field.GetValue(null);
                    return value == null ? default : (TEnum)value;
                }
            }

            throw new InvalidEnumArgumentException($"Enum member with description/name '{description}', not found!");
        }

        /// <summary>
        /// Return enum value based on the given Name property of Display attribute. 
        /// If no Name is set on a field, then field name will be compared aginst the given displayName
        /// </summary>
        /// <typeparam name="TEnum">typeof enum</typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static List<TEnum> GetValueFromDisplayName<TEnum>(string displayName) where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new InvalidOperationException();

            List<TEnum> results = [];

            foreach (var field in type.GetFields())
            {
                //Is there any Description attribute set for the field?
                if (
                    string.Equals(
                        Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute ? attribute.Name : field.Name,
                        displayName,
                        StringComparison.CurrentCultureIgnoreCase)
                    )
                {
                    var value = field.GetValue(null);
                    results.Add(value == null ? default : (TEnum)value);
                }
            }
            if (results.Any())
                return results;

            throw new InvalidEnumArgumentException($"Enum member with displayName/name '{displayName}', not found!");
        }
    }
}