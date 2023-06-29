using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Jinget.Core.ExtensionMethods.Database.SqlClient;
using System.Data.SqlClient;
using System;

namespace Jinget.Core.Tests.ExtensionMethods.Database.SqlClient
{
    [TestClass]
    public class IDbCommandExtensionsTests
    {
        [TestMethod()]
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

            Assert.IsFalse(command.CommandText.Contains("علي"));
            Assert.IsTrue(command.CommandText.Contains("علی"));

            Assert.IsFalse(command.CommandText.Contains("روشنك"));
            Assert.IsTrue(command.CommandText.Contains("روشنک"));

            Assert.IsFalse(((SqlParameter)command.Parameters["@p1"]).Value.ToString() == "N'رهي'");
            Assert.IsTrue(((SqlParameter)command.Parameters["@p1"]).Value.ToString() == "N'رهی'");

            Assert.IsFalse(((SqlParameter)command.Parameters["@p2"]).Value.ToString() == "N'قاصدك'");
            Assert.IsTrue(((SqlParameter)command.Parameters["@p2"]).Value.ToString() == "N'قاصدک'");
        }
    }
}
