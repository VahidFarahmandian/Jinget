namespace Jinget.Core.Security.SqlInjection
{
    /// <summary>
    /// SqlInjection preventation class
    /// </summary>
    public static class SqlInjection
    {
        /// <summary>
        /// Immunize the given string and remove the dangerouse characters from this string
        /// </summary>
        /// <param name="replaceTagQuoteAndSemiColon">if set to <c>true</c> tag quote and semicolon characters will be removed from the input string</param>
        public static string SecureString(this string insecureString, bool replaceTagQuoteAndSemiColon = true)
        {
            if (replaceTagQuoteAndSemiColon)
            {
                insecureString = insecureString
                    .Replace("'", "''")
                    .Replace(";", string.Empty);
            }

            return insecureString
                .Replace("--", string.Empty)
                .Replace("<script", string.Empty)
                .Replace("script>", string.Empty)
                .Trim();
        }
    }
}