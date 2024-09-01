using Jinget.Core.Utilities.Parser.DatabaseParser.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinget.Core.Tests.Utilities.Parser.DatabaseParser.SqlServer;

[TestClass]
public class SqlServerParserUtilityTests
{
    [TestMethod()]
    public void should_parse_connectionstring()
    {
        string connectionString = "Server=YOURSERVERNAME; Database=YOURDATABASENAME; Trusted_Connection=True; MultipleActiveResultSets=true";

        var result = SqlServerParserUtility.ParseConnectionString(connectionString);

        Assert.IsTrue(result.ContainsKey("Server"));
        Assert.IsTrue(result.ContainsKey("Database"));
        Assert.IsTrue(result.ContainsKey("Trusted_Connection"));
        Assert.IsTrue(result.ContainsKey("MultipleActiveResultSets"));
    }

    [TestMethod()]
    public void should_return_empty_result()
    {
        string invalidConnectionString = "InvalidConnectionString";
        var result = SqlServerParserUtility.ParseConnectionString(invalidConnectionString);
        Assert.IsTrue(result.Keys.Count == 0);

        string emptyConnectionString = "";
        result = SqlServerParserUtility.ParseConnectionString(emptyConnectionString);
        Assert.IsTrue(result.Keys.Count == 0);

        string nullConnectionString = "";
        result = SqlServerParserUtility.ParseConnectionString(nullConnectionString);
        Assert.IsTrue(result.Keys.Count == 0);
    }
}
