namespace Jinget.Core.Utilities.Enum;

public static class EnumUtility
{
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

    /// <summary>
    /// Get the minimum value in enum
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <typeparam name="TValue">enum data type</typeparam>
    public static TValue GetMinValue<TEnum, TValue>() where TValue : struct, IConvertible
    {
        
        var enumValues = System.Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        if (enumValues.Any())
        {
            var minVal = enumValues.First();
            return (TValue)Convert.ChangeType(minVal, typeof(TValue));
        }
        else
            throw new InvalidEnumArgumentException("Enum is empty and contains no value");
    }

    /// <summary>
    /// Get the maximum value in enum
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <typeparam name="TValue">enum data type</typeparam>
    public static TValue GetMaxValue<TEnum, TValue>() where TValue : struct, IConvertible
    {
        var enumValues = System.Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        if (enumValues.Any())
        {
            var maxVal = enumValues.Last();
            return (TValue)Convert.ChangeType(maxVal, typeof(TValue));
        }
        else
            throw new InvalidEnumArgumentException("Enum is empty and contains no value");
    }
}