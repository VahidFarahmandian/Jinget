using System.Linq;

namespace Jinget.Core.ExtensionMethods
{
    public static class StringExtensions
    {
        static readonly char[] ArabicYeChar = { (char)1610, 'ي'};
        const char PersianYeChar = (char)1740;

        static readonly char[] ArabicKeChar = { (char)1603, 'ك' };
        const char PersianKeChar = (char)1705;

        /// <summary>
        /// Replace Arabic ي and ك characters with their Farsi equalivants
        /// </summary>
        public static string ApplyCorrectYeKe(this string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return string.Empty;

            data = ArabicYeChar.Aggregate(data, (current, ye) => current.Replace(ye, PersianYeChar).Trim());

            data = ArabicKeChar.Aggregate(data, (current, ke) => current.Replace(ke, PersianKeChar).Trim());

            return data;
        }
    }
}