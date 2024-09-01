using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jinget.Core.Tests._BaseData;
using Jinget.Core.ExtensionMethods.Reflection;

namespace Jinget.Core.Tests.ExtensionMethods.Reflection;

[TestClass()]
public class MethodInfoExtensionsTests
{
    async Task<List<Type1>> GetAsync(string name) => await Task.FromResult<List<Type1>>([
        new() {Id = 1, Name=name},
        new() {Id = 2, Name="Ali"}
    ]);
    [TestMethod()]
    public void Should_call_async_method_and_return_result()
    {
        var method = GetType().GetMethod("GetAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = (List<Type1>)method.InvokeAsync(this, "vahid");

        Assert.IsTrue(result.First().Id == 1);
        Assert.IsTrue(result.First().Name == "vahid");
        Assert.IsTrue(result.Last().Id == 2);
    }
}