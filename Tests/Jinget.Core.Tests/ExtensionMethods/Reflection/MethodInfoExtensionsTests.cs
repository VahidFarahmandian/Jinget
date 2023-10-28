using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.ExtensionMethods.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Jinget.Core.Tests._BaseData;

namespace Jinget.Core.ExtensionMethods.Reflection.Tests
{
    [TestClass()]
    public class MethodInfoExtensionsTests
    {
        async Task<List<Type1>> GetAsync(string name) => new List<Type1> {
            new Type1 {Id = 1, Name=name},
            new Type1 {Id = 2, Name="Ali"}
        };
        [TestMethod()]
        public void Should_call_async_method_and_return_result()
        {
            var method = this.GetType().GetMethod("GetAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (List<Type1>)method.InvokeAsync(this, "vahid");

            Assert.IsTrue(result.First().Id == 1);
            Assert.IsTrue(result.First().Name == "vahid");
            Assert.IsTrue(result.Last().Id == 2);
        }
    }
}