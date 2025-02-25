using System.Text.Json;

namespace Jinget.Core.Tests.Utilities.Json;

[TestClass]
public class IgnorePropertiesResolverTests
{
    [TestMethod]
    public void should_ignore_given_properties_while_serialization()
    {
        string expected = "{\"Property1\":1,\"Property4\":false,\"InnerSingularProperty\":null,\"InnerProperty\":null,\"InnerListProperty\":null}";

        var options = new JsonSerializerOptions();
        options.Converters.Add(new IgnorePropertiesResolver<_BaseData.TestClass>(new[] { nameof(_BaseData.TestClass.Property2), nameof(_BaseData.TestClass.Property3) }));

        var result = new _BaseData.TestClass
        {
            Property1 = 1
        }.Serialize(options);

        Assert.AreEqual(expected, result);
    }
}
