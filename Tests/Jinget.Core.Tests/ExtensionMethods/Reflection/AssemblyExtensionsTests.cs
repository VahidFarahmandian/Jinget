using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Jinget.Core.Tests._BaseData;

namespace Jinget.Core.ExtensionMethods.Reflection.Tests;

[TestClass()]
public class AssemblyExtensionsTests
{
    [TestMethod()]
    public void should_get_all_types_in_an_assembly()
    {
        var result = AssemblyExtensions.GetTypes(this.GetType().Assembly, typeof(NonGenericParent), normalizingPattern: @"Parent`1$");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Assert.IsTrue(result.All(x => x.Summary != string.Empty));
    }

    [TestMethod()]
    public void should_get_all_authorized_methods_in_a_type()
    {
        var result = AssemblyExtensions.GetMethods(this.GetType().Assembly, typeof(NonGenericParent), typeof(Attributes.SummaryAttribute));
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Assert.IsTrue(result.Any(x => x.MethodName!= "SampleMethod3"));
    }


    [TestMethod()]
    public void should_get_all_methods_in_a_type()
    {
        var result = AssemblyExtensions.GetMethods(this.GetType().Assembly, typeof(NonGenericParent), typeof(Attributes.SummaryAttribute), onlyAuthorizedMethods: false);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Assert.IsTrue(result.Any(x => x.Summary != null));
    }
}