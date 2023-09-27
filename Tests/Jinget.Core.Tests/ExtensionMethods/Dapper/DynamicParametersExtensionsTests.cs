using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Dapper;

namespace Jinget.Core.ExtensionMethods.Dapper.Tests
{
    [TestClass()]
    public class DynamicParametersExtensionsTests
    {
        [TestMethod()]
        public void should_get_key_value_pairs_for_dynamicparameter()
        {
            DynamicParameters parameters = new();
            parameters.Add("persianName", "وحید", System.Data.DbType.String);
            parameters.Add("englishName", "vahid", System.Data.DbType.AnsiString);
            parameters.Add("sampleNumber", 123, System.Data.DbType.Int32);
            parameters.Add("sampleBool", true, System.Data.DbType.Boolean);

            var result = parameters.GetKeyValues();

            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result.GetValueOrDefault("persianName") == "وحید");
            Assert.IsTrue(result.GetValueOrDefault("englishName") == "vahid");
            Assert.IsTrue(result.GetValueOrDefault("sampleNumber") == 123);
            Assert.IsTrue(result.GetValueOrDefault("sampleBool") == true);
        }

        [TestMethod()]
        public void should_convert_values_to_sql_values()
        {
            DynamicParameters parameters = new();
            parameters.Add("persianName", "وحید", System.Data.DbType.String);
            parameters.Add("englishName", "vahid", System.Data.DbType.AnsiString);
            parameters.Add("sampleNumber", 123, System.Data.DbType.Int32);
            parameters.Add("sampleBool", true, System.Data.DbType.Boolean);
            var result = parameters.GetSQLValues();

            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result[0] == "N'وحید'");
            Assert.IsTrue(result[1] == "'vahid'");
            Assert.IsTrue(result[2] == 123);
            Assert.IsTrue(result[3] == 1);
        }
    }
}