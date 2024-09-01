namespace Jinget.Core.Utilities.Parser.DatabaseParser;

public static class DatabaseParserUtility
{
    public static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
        string[] parts = connectionString.Split(";", StringSplitOptions.RemoveEmptyEntries);
        var keyValuePairs = new Dictionary<string, string>();
        foreach (var item in parts)
        {
            var keyValues = item.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (keyValues.Length == 2)
                keyValuePairs.Add(keyValues[0].Trim(), keyValues[1].Trim());
        }
        return keyValuePairs;
    }
}