namespace Jinget.Core.Security.SqlInjection;

/// <summary>
/// SqlInjection preventation class
/// </summary>
public static class SqlInjection
{
    /// <summary>
    /// Immunize the given string and remove the dangerouse characters from this string
    /// </summary>
    /// <param name="replaceTagQuoteAndSemiColon">if set to <c>true</c> tag quote and semicolon characters will be removed from the input string</param>
    /// <param name="removeHtmlTags">if set to <c>true</c> html tags will be removed from the input string</param>
    public static string SecureString(this string insecureString, bool replaceTagQuoteAndSemiColon = true, bool removeHtmlTags = true)
    {
        StringBuilder sb = new(insecureString);

        if (replaceTagQuoteAndSemiColon)
        {
            sb.Replace("'", "''")
              .Replace(";", string.Empty);
        }

        if (removeHtmlTags)
        {
            Regex regex = new("<.*?>");
            var tags = regex.Matches(sb.ToString()).ToList();
            tags.ForEach(p => sb.Replace(p.Value, string.Empty));
        }

        return sb
            .Replace("--", string.Empty).ToString()
            .Trim();
    }
}