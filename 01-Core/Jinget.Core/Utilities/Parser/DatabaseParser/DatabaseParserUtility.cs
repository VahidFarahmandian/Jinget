using System.Collections.Generic;

namespace Jinget.Core.Utilities.Parser.SqlServer
{
    public static class DatabaseParserUtility
    {
        public static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            string[] parts = connectionString.Split(";", System.StringSplitOptions.RemoveEmptyEntries);
            var keyValuePairs = new Dictionary<string, string>();
            foreach (var item in parts)
            {
                var keyValues = item.Split('=', System.StringSplitOptions.RemoveEmptyEntries);
                if (keyValues.Length == 2)
                    keyValuePairs.Add(keyValues[0].Trim(), keyValues[1].Trim());
            }
            return keyValuePairs;
        }
    }
}