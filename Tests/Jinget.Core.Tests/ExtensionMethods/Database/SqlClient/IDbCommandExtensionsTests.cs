namespace Jinget.Core.Tests.ExtensionMethods.Database.SqlClient;

[TestClass]
public class IDbCommandExtensionsTests
{
    [TestMethod]
    public void should_replace_arabic_YeKe_in_sqlcommand_with_its_farsi_equalivants()
    {
        IDbCommand command = new SqlCommand
        {
            CommandText = "SELECT * FROM dbo.Users Where (Username = N'علي' OR Username = N'روشنك' OR UserName = @p1 OR UserName = @p2) AND UserName <> @p3"
        };

        command.Parameters.Add(new SqlParameter("@p1", "N'رهي'"));
        command.Parameters.Add(new SqlParameter("@p2", "N'قاصدك'"));
        command.Parameters.Add(new SqlParameter("@p3", DBNull.Value));

        command.ApplyCorrectYeKe();

        Assert.DoesNotContain("علي", command.CommandText);
        Assert.Contains("علی", command.CommandText);

        Assert.DoesNotContain("روشنك", command.CommandText);
        Assert.Contains("روشنک", command.CommandText);

        Assert.AreNotEqual("N'رهي'", ((SqlParameter)command.Parameters["@p1"]).Value.ToString());
        Assert.AreEqual("N'رهی'", ((SqlParameter)command.Parameters["@p1"]).Value.ToString());

        Assert.AreNotEqual("N'قاصدك'", ((SqlParameter)command.Parameters["@p2"]).Value.ToString());
        Assert.AreEqual("N'قاصدک'", ((SqlParameter)command.Parameters["@p2"]).Value.ToString());
    }
}
