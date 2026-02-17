using System.Data.Common;
using System.Threading;

namespace Jinget.Core.Tests.ExtensionMethods.Database.SqlClient;

[TestClass]
public class DbConnectionExtensionsTests
{
    Mock<DbConnection> mockDbConnection;
    [TestInitialize]
    public void Initialize()
    {
        ConnectionState state = ConnectionState.Closed;

        mockDbConnection = new Mock<DbConnection>();
        // Setup OpenAsync to change state to Open
        mockDbConnection.Setup(x => x.OpenAsync(It.IsAny<CancellationToken>()))
            .Callback(() => state = ConnectionState.Open)
            .Returns(Task.CompletedTask);

        // Setup CloseAsync to change state to Closed
        mockDbConnection.Setup(x => x.CloseAsync())
            .Callback(() => state = ConnectionState.Closed)
            .Returns(Task.CompletedTask);

        // Setup State getter to return currentState
        mockDbConnection.Setup(x => x.State).Returns(() => state);

    }
    [TestMethod]
    public async Task Should_return_open_connection_statusAsync()
    {
        var cnn = mockDbConnection.Object;
        await cnn.SafeOpenAsync();
        await cnn.SafeOpenAsync();
        Assert.IsTrue(cnn.State == ConnectionState.Open);
    }

    [TestMethod]
    public void should_create_query_for_generic_order_by_message()
    {
        var select = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest");
        var param = new GenericRequestSampleMessage();
        var result = DbConnectionExtensions.PrepareQuery(select, param);

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.queryText));
    }

    [TestMethod]
    public void should_create_query_for_nongeneric_order_by_message()
    {
        var select = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest");
        var param = new NonGenericRequestSampleMessage();
        var result = DbConnectionExtensions.PrepareQuery(select, param);

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.queryText));
    }

    [TestMethod]
    public void should_create_same_query_for_different_order_by_type_same_message()
    {
        var select = Sql.Select<SqlTableSample, object>(x => new { x.Id }, "tblTest");

        var param1 = new GenericRequestSampleMessage();
        var result1 = DbConnectionExtensions.PrepareQuery(select, param1);

        var param2 = new NonGenericRequestSampleMessage();
        var result2 = DbConnectionExtensions.PrepareQuery(select, param2);

        Assert.AreEqual(result1.queryText, result2.queryText);
    }
}
