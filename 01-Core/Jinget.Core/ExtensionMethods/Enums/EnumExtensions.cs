namespace Jinget.Core.ExtensionMethods.Enums;

public static class EnumExtensions
{
    /// <summary>
    /// Get Name property of Display attribute for a specific enum value
    /// If no Name is set on a field, then the stringfied value will be returned
    /// </summary>
    public static string? GetDisplayName(this Enum value)
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
}