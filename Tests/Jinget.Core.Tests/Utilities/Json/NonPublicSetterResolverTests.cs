using System.Text.Json;

namespace Jinget.Core.Tests.Utilities.Json;

[TestClass]
public class NonPublicSetterResolverTests
{
    [TestMethod]
    public void should_ignore_given_properties_while_serialization()
    {
        string expected = "{\"Id\":1,\"Name\":\"Vahid\"}";

        var options = new JsonSerializerOptions();
        options.Converters.Add(new NonPublicSetterConverter<TestClass>());

        var result = new _BaseData.ClassWithNonPublicSetterProps("Vahid")
        {
            Id = 1
        }.Serialize(options);

        Assert.AreEqual(expected, result);
    }
}
