using System.Text.Json;

namespace Jinget.Core.Utilities.Json;


/// <summary>
/// A custom <see cref="JsonNamingPolicy"/> that converts property names to pascal-case.
/// </summary>
/// <remarks>
/// This policy converts property names as follows:
/// <list type="bullet">
///     <item>
///         <term>Null or empty strings</term>
///         <description>Remain unchanged.</description>
///     </item>
///     <item>
///         <term>All uppercase strings</term>
///         <description>Are converted to pascal-case.</description>
///     </item>
///     <item>
///         <term>Strings with a pascal-case first character</term>
///         <description>Remain unchanged.</description>
///     </item>
///     <item>
///         <term>Strings with an uppercase first character</term>
///         <description>The first character is converted to pascal-case, and the rest remains unchanged.</description>
///     </item>
/// </list>
/// </remarks>
public class PascalCaseNamingPolicy : JsonNamingPolicy
{
    /// <summary>
    /// Converts the specified name to pascal-case based on the defined rules.
    /// </summary>
    /// <param name="name">The name to convert.</param>
    /// <returns>The converted name in pascal-case.</returns>
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        if (name.All(char.IsUpper))
            return name.ToLower();

        if (!char.IsUpper(name[0]))
            return name;

        return char.ToLower(name[0]) + name.Substring(1);
    }
}