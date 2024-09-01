using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using Jinget.Core.Exceptions;
using Jinget.Core.Tests._BaseData;
using Jinget.Core.ExtensionMethods.Expressions;

namespace Jinget.Core.Tests.ExtensionMethods.Expressions;

[TestClass()]
public class LambdaExpressionExtensionsTests
{
    [TestMethod()]
    public void stringfy_constant_exppression()
    {
        Expression<Func<TestClass, object>> expr = x => 1;
        var result = expr.Stringfy();
        Assert.AreEqual("1", result);
    }

    [TestMethod()]
    public void stringfy_member_exppression()
    {
        Expression<Func<TestClass, object>> expr = x => x.Property1;
        var result = expr.Stringfy();
        Assert.AreEqual("Property1", result);
    }

    [TestMethod()]
    public void stringfy_methodcall_select_exppression()
    {
        Expression<Func<TestClass, object>> expr = x => x.InnerProperty.Select(y => y.InnerProperty1);
        var result = expr.Stringfy();
        Assert.AreEqual("InnerProperty.InnerProperty1", result);
    }

    [TestMethod()]
    public void stringfy_methodcall_tostring_exppression()
    {
        Expression<Func<TestClass, object>> expr = x => x.Property2.ToString();
        var result = expr.Stringfy();
        Assert.AreEqual("Property2", result);
    }

    [TestMethod()]
    public void stringfy_methodcall_tolower_exppression()
    {
        Expression<Func<TestClass, object>> expr = x => x.Property2.ToLower();
        var result = expr.Stringfy();
        Assert.AreEqual("Property2", result);
    }

    [TestMethod()]
    [ExpectedException(typeof(JingetException))]
    public void stringfy_methodcall_where_exppression()
    {
        Expression<Func<TestClass, object>> expr = x => x.InnerProperty.Where(y => x.Property1 > 0);
        var result = expr.Stringfy();
    }

    [TestMethod()]
    [ExpectedException(typeof(JingetException))]
    public void stringfy_methodcall_orderby_exppression()
    {
        Expression<Func<TestClass, object>> expr = x => x.InnerProperty.OrderBy(y => x.Property1);
        var result = expr.Stringfy();
    }
}