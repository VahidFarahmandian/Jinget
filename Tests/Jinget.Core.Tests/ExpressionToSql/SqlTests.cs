using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.Tests._BaseData;
using Jinget.Core.ExpressionToSql.Internal;
using System.Collections.Generic;
using System;

namespace Jinget.Core.ExpressionToSql.Tests
{
    [TestClass()]
    public class SqlTests
    {
        #region SELECT

        #region Simple Select

        [TestMethod]
        public void Should_return_select_from_table_using_table_name()
        {
            var expectedResult = "SELECT a.[Id] FROM [dbo].[tblTest] AS a";

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").ToSql();

            Assert.AreEqual(expectedResult, result.query.ToString());
            Assert.IsTrue(result.parameters is null);
        }

        [TestMethod]
        public void Should_return_select_from_table_using_table_type()
        {
            var expectedResult = "SELECT a.[Id] FROM [dbo].[tblTest] AS a";

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, new Table { Name = "tblTest" }).ToSql();

            Assert.AreEqual(expectedResult, result.query.ToString());
            Assert.IsTrue(result.parameters is null);
        }

        #endregion

        #region Custome Schema

        [TestMethod]
        public void Should_return_select_single_column_from_table_custom_schema()
        {
            var expectedResult = "SELECT a.[Id] FROM [sch].[tblTest] AS a";

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, new Table() { Name = "tblTest", Schema = "sch" }).ToSql();

            Assert.AreEqual(expectedResult, result.query.ToString());
            Assert.IsTrue(result.parameters is null);
        }

        #endregion

        #region TOP
        [TestMethod]
        public void Should_return_select_top_from_table_using_table_name()
        {
            var expectedResult = "SELECT TOP 10 a.[Id] FROM [dbo].[tblTest] AS a";

            var result = Sql.Top<SqlTableSample, object>(x => new { x.Id }, 10, "tblTest").ToSql();

            Assert.AreEqual(expectedResult, result.query.ToString());
            Assert.IsTrue(result.parameters is null);
        }

        [TestMethod]
        public void Should_return_select_top_from_table_using_table_type()
        {
            var expectedResult = "SELECT TOP 10 a.[Id] FROM [dbo].[tblTest] AS a";

            var result = Sql.Top<SqlTableSample, object>(x => new { x.Id }, 10, new Table { Name = "tblTest" }).ToSql();

            Assert.AreEqual(expectedResult, result.query.ToString());
            Assert.IsTrue(result.parameters is null);
        }

        #endregion

        #region Multiple Column

        [TestMethod]
        public void Should_return_select_multiple_columns_from_table()
        {
            var expectedQuery = "SELECT a.[Id], a.[FirstName] FROM [dbo].[tblTest] AS a";

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id, x.FirstName }, "tblTest").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());
        }

        [TestMethod]
        public void Should_return_select_all_columns_from_table()
        {
            var expectedQuery = "SELECT a.[Id], a.[FirstName], a.[LastName], a.[Age] FROM [dbo].[tblTest] AS a";

            var result = Sql.Select<SqlTableSample, object>(x => x, "tblTest").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());
        }

        [TestMethod]
        public void Should_return_select_one_column_and_one_constant_from_table()
        {
            var expectedQuery = "SELECT a.[Id], 'Const Value' FROM [dbo].[tblTest] AS a";

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id, Name = "'Const Value'" }, "tblTest").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());
        }

        #endregion

        #endregion

        #region WHERE

        #region WHERE =

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_string_column_equal()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([FirstName] = @1)";
            Dictionary<string, object> expectedParameters = new() { { "1", "Vahid" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName == "Vahid").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 1)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_string_column_equal_to_itself()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([FirstName] = @FirstName)";
            Dictionary<string, object> expectedParameters = new();

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName == x.FirstName).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 0)
                Assert.Fail($"Expected 0 parameter, but get {expectedParameters.Count}");
        }

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_string_column_equal_to_other_column()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([FirstName] = @LastName)";
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName == x.LastName).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 0)
                Assert.Fail($"Expected 0 parameter, but get {expectedParameters.Count}");
        }

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_string_column_call_method()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([FirstName] = @1)";
            Dictionary<string, object> expectedParameters = new() { { "1", "vahid" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName == "Vahid".ToLower()).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 1)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_exception_for_unsupported_method_call()
        {
            Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName.Normalize() == "Vahid").ToSql();
        }

        #endregion

        #region Where LIKE

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_string_column_like()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([FirstName] LIKE @1)";
            Dictionary<string, object> expectedParameters = new() { { "1", "%vahid%" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName.Contains("vahid")).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 1)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_string_column_like_start()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([FirstName] LIKE @1)";
            Dictionary<string, object> expectedParameters = new() { { "1", "vahid%" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName.StartsWith("vahid")).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 1)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_string_column_like_end()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([FirstName] LIKE @1)";
            Dictionary<string, object> expectedParameters = new Dictionary<string, object> { { "1", "%vahid" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName.EndsWith("vahid")).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 1)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        #endregion

        #region Where >

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_number_column_greaterthan()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([Age] > @1)";
            Dictionary<string, object> expectedParameters = new() { { "1", 20 } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.Age > 20).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 1)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        #endregion

        #region Where IN

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_number_column_in()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ([Age] IN (@1,@2))";
            Dictionary<string, object> expectedParameters = new() { { "1", 20 }, { "2", 30 } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => new List<int>() { 20, 30 }.Contains(x.Age)).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 2)
                Assert.Fail($"Expected 2 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            if (!result.parameters.ContainsKey("2"))
                Assert.Fail("Expected parameter 2 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
            Assert.AreEqual(expectedParameters["2"].ToString(), result.parameters["2"].ToString());
        }

        #endregion

        #region WHERE AND =
        [TestMethod]
        public void Should_return_select_single_column_from_table_where_two_string_columns_and()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE (([FirstName] = @1) AND ([LastName] = @2))";
            Dictionary<string, object> expectedParameters = new() { { "1", "Vahid" }, { "2", "Farahmandian" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName == "Vahid" && x.LastName == "Farahmandian").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 2)
                Assert.Fail($"Expected 2 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            if (!result.parameters.ContainsKey("2"))
                Assert.Fail("Expected parameter 2 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());

            Assert.AreEqual(expectedParameters["2"].ToString(), result.parameters["2"].ToString());
        }

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_one_string_column_and_one_number_column()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE (([Age] > @1) AND ([LastName] = @2))";
            Dictionary<string, object> expectedParameters = new() { { "1", 20 }, { "2", "Farahmandian" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.Age > 20 && x.LastName == "Farahmandian").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 2)
                Assert.Fail($"Expected 2 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            if (!result.parameters.ContainsKey("2"))
                Assert.Fail("Expected parameter 2 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());

            Assert.AreEqual(expectedParameters["2"].ToString(), result.parameters["2"].ToString());
        }

        #endregion

        #region WHERE OR

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_two_string_columns_or()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE (([FirstName] = @1) OR ([LastName] = @2))";
            Dictionary<string, object> expectedParameters = new() { { "1", "Vahid" }, { "2", "Farahmandian" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName == "Vahid" || x.LastName == "Farahmandian").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 2)
                Assert.Fail($"Expected 2 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            if (!result.parameters.ContainsKey("2"))
                Assert.Fail("Expected parameter 2 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());

            Assert.AreEqual(expectedParameters["2"].ToString(), result.parameters["2"].ToString());
        }

        public string SampleProperty { get; set; }
        [TestMethod]
        public void Should_return_select_single_column_from_table_where_two_string_columns_equal_to_field_and_property_value()
        {
            string sampleField = "testFieldValue";
            SampleProperty = "testPropertyValue";
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE (([FirstName] = @1) OR ([LastName] = @2))";
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>() { { "1", "testFieldValue" }, { "2", "testPropertyValue" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.FirstName == sampleField || x.LastName == SampleProperty).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 2)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        #endregion

        #region AND OR

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_two_char_columns_and_or()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ((([FirstName] = @1) OR ([LastName] = @2)) AND (([LastName] = @3) OR ([FirstName] = @4)))";
            Dictionary<string, object> expectedParameters = new() { { "1", "Vahid" }, { "2", "Farahmandian" }, { "3", "Vahid" }, { "4", "Farahmandian" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest")
                .Where(x => (x.FirstName == "Vahid" || x.LastName == "Farahmandian") && (x.LastName == "Vahid" || x.FirstName == "Farahmandian")).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 4)
                Assert.Fail($"Expected 4 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            if (!result.parameters.ContainsKey("2"))
                Assert.Fail("Expected parameter 2 not found");

            if (!result.parameters.ContainsKey("3"))
                Assert.Fail("Expected parameter 3 not found");

            if (!result.parameters.ContainsKey("4"))
                Assert.Fail("Expected parameter 4 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());

            Assert.AreEqual(expectedParameters["2"].ToString(), result.parameters["2"].ToString());

            Assert.AreEqual(expectedParameters["3"].ToString(), result.parameters["3"].ToString());

            Assert.AreEqual(expectedParameters["4"].ToString(), result.parameters["4"].ToString());
        }

        #endregion

        #region WHERE CAST

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_cast_convert_tostring()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE (CAST(FirstName AS NVARCHAR(MAX)) = @1)";
            Dictionary<string, object> expectedParameters = new() { { "1", "vahid" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => Convert.ToString(x.FirstName) == "Vahid".ToLower()).ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 1)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_cast_tostring()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE (CAST(Id AS NVARCHAR(MAX)) = @1)";
            Dictionary<string, object> expectedParameters = new() { { "1", "Vahid" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest").Where(x => x.Id.ToString() == "Vahid").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 1)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
        }

        #endregion

        #region WHERE UPPER & LOWER

        [TestMethod]
        public void Should_return_select_single_column_from_table_where_upper()
        {
            var expectedQuery = "SELECT a.[Id] FROM [dbo].[tblTest] AS a WHERE ((UPPER(FirstName) = @1) AND (LOWER(LastName) = @2))";
            Dictionary<string, object> expectedParameters = new() { { "1", "Vahid" }, { "2", "farahmandian" } };

            var result = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest")
                .Where(x => x.FirstName.ToUpper() == "Vahid" && x.LastName.ToLower() == "farahmandian").ToSql();

            Assert.AreEqual(expectedQuery, result.query.ToString());

            Assert.IsNotNull(expectedParameters);

            if (result.parameters.Count != 2)
                Assert.Fail($"Expected 1 parameter, but get {expectedParameters.Count}");

            if (!result.parameters.ContainsKey("1"))
                Assert.Fail("Expected parameter 1 not found");

            if (!result.parameters.ContainsKey("2"))
                Assert.Fail("Expected parameter 2 not found");

            Assert.AreEqual(expectedParameters["1"].ToString(), result.parameters["1"].ToString());
            Assert.AreEqual(expectedParameters["2"].ToString(), result.parameters["2"].ToString());

        }

        #endregion

        #endregion
    }
}