using Jinget.Core.Utilities.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Jinget.Core.Tests.Utilities.Json
{
    [TestClass()]
    public class IgnorePropertiesResolverTests
    {
        [TestMethod()]
        public void should_ignore_given_properties_while_serialization()
        {
            string expected = "{\"Property1\":1,\"Property4\":false,\"InnerSingularProperty\":null,\"InnerProperty\":null,\"InnerListProperty\":null}";

            var result = JsonConvert.SerializeObject(new _BaseData.TestClass
            {
                Property1 = 1
            },
            new JsonSerializerSettings()
            {
                ContractResolver = new IgnorePropertiesResolver(new[] { nameof(_BaseData.TestClass.Property2), nameof(_BaseData.TestClass.Property3) })
            });

            Assert.AreEqual(expected, result);
        }
    }
}
