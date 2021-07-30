using Jinget.Extensions.Database.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;

namespace Jinget.Extensions.Tests.Database.SqlClientTests
{
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
    }
}
