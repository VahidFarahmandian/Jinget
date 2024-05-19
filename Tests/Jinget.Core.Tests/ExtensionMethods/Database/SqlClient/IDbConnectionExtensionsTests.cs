using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using Jinget.Core.ExtensionMethods.Database.SqlClient;
using Jinget.Core.ExpressionToSql;
using Jinget.Core.Tests._BaseData;

namespace Jinget.Core.Tests.ExtensionMethods.Database.SqlClient;

[TestClass]
public class IDbConnectionExtensionsTests
{
    Mock<IDbConnection> mockDbConnection;
    [TestInitialize]
    public void Initialize()
    {
        ConnectionState state = ConnectionState.Closed;

        mockDbConnection = new Mock<IDbConnection>();
        mockDbConnection.Setup(x => x.Open()).Callback(() => { state = ConnectionState.Open; });
        mockDbConnection.Setup(x => x.Close()).Callback(() => { state = ConnectionState.Closed; });
        mockDbConnection.Setup(x => x.State).Returns(() => state);

    }
    [TestMethod()]
    public void Should_return_open_connection_status()
    {
        var cnn = mockDbConnection.Object;
        cnn.SafeOpen();
        cnn.SafeOpen();
        Assert.IsTrue(cnn.State == ConnectionState.Open);
    }

    [TestMethod()]
    public void should_create_query_for_generic_order_by_message()
    {
        var select = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest");
        var param = new GenericRequestSampleMessage();
        var result = IDbConnectionExtensions.PrepareQuery(select, param);

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.queryText));
    }

    [TestMethod()]
    public void should_create_query_for_nongeneric_order_by_message()
    {
        var select = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest");
        var param = new NonGenericRequestSampleMessage();
        var result = IDbConnectionExtensions.PrepareQuery(select, param);

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.queryText));
    }

    [TestMethod()]
    public void should_create_same_query_for_different_order_by_type_same_message()
    {
        var select = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest");

        var param1 = new GenericRequestSampleMessage();
        var result1 = IDbConnectionExtensions.PrepareQuery(select, param1);

        var param2 = new NonGenericRequestSampleMessage();
        var result2 = IDbConnectionExtensions.PrepareQuery(select, param2);

        Assert.AreEqual(result1.queryText, result2.queryText);
    }
}
