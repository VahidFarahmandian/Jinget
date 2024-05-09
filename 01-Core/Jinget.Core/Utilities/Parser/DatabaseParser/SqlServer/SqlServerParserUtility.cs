using System.Collections.Generic;

namespace Jinget.Core.Utilities.Parser.DatabaseParser.SqlServer;

public static class SqlServerParserUtility
{
    public static Dictionary<string, string> ParseConnectionString(string connectionString) => DatabaseParserUtility.ParseConnectionString(connectionString);
}