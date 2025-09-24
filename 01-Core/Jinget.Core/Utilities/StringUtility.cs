namespace Jinget.Core.Utilities;

/// <summary>
/// Provides utility methods for string manipulation and generation.
/// </summary>
public static class StringUtility
{
    private const string DefaultCharSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    /// <summary>
    /// Generates a random string of a specified length using a given character set.
    /// </summary>
    /// <param name="length">The length of the random string to generate.</param>
    /// <param name="characterSet">The set of characters to use for generating the random string.</param>
    /// <returns>The generated random string.</returns>
    /// <exception cref="ArgumentException">Thrown when the length is negative or too large, or when the character set is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the character set is null.</exception>
    public static string GetRandomString(int length, IEnumerable<char>? characterSet = null)
    {
        if (length < 0)
            throw new ArgumentException("Jinget Says: length must not be negative", nameof(length));
        if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
            throw new ArgumentException("Jinget Says: length is too big", nameof(length));

        if (characterSet == null || characterSet.ToArray().Length == 0)
            characterSet = [.. DefaultCharSet];

        var characterArray = characterSet.Distinct().ToArray();

        var bytes = new byte[length * 8];
        var result = new char[length];
        using (var cryptoProvider = RandomNumberGenerator.Create())
        {
            cryptoProvider.GetBytes(bytes);
        }
        for (int i = 0; i < length; i++)
        {
            ulong value = BitConverter.ToUInt64(bytes, i * 8);
            result[i] = characterArray[value % (uint)characterArray.Length];
        }
        return new string(result);
    }

    /// <summary>
    /// Checks if a given string contains only numeric characters.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>True if the string contains only digits, otherwise false.</returns>
    public static bool IsDigitOnly(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        foreach (char c in input)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets a string containing English alphabet characters.
    /// </summary>
    /// <param name="lowerCase">Indicates whether to return lowercase or uppercase characters.</param>
    /// <returns>A string containing English alphabet characters.</returns>
    public static string GetEnglishChars(bool lowerCase) => lowerCase ? "abcdefghijklmnopqrstuvwxyz" : "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Gets a string containing English digits.
    /// </summary>
    /// <returns>A string containing English digits.</returns>
    public static string GetEnglishDigits() => "1234567890";

    /// <summary>
    /// Gets a list of Farsi characters.
    /// </summary>
    /// <returns>A list of Farsi characters.</returns>
    public static List<char> GetFarsiChars() => ['ا', 'ب', 'پ', 'ت', 'ث', 'ج', 'چ', 'ح', 'خ', 'د', 'ذ', 'ر', 'ز', 'ژ', 'س', 'ش', 'ص', 'ض', 'ط', 'ظ', 'ع', 'غ', 'ف', 'ق', 'ک', 'گ', 'ل', 'م', 'ن', 'و', 'ه', 'ی'];

    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Split on any whitespace, remove empties, then rejoin with a single space
        return string.Join(" ", input.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
    }

}