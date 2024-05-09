using Jinget.Core.Utilities.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Jinget.Core.Tests.Utilities.Json;

[TestClass()]
public class NonPublicSetterResolverTests
{
    [TestMethod()]
    public void should_ignore_given_properties_while_serialization()
    {
        string expected = "{\"Id\":1,\"Name\":\"Vahid\"}";

        var result = JsonConvert.SerializeObject(new _BaseData.ClassWithNonPublicSetterProps("Vahid")
        {
            Id = 1
        },
        new JsonSerializerSettings()
        {
            ContractResolver = new NonPublicSetterResolver()
        });

        Assert.AreEqual(expected, result);
    }
}
