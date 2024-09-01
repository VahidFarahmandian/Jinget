using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinget.Core.Tests.Security.SqlInjection;

[TestClass()]
public class SqlInjectionTests
{
    [TestMethod()]
    public void should_immunize_string()
    {
        string input = "<script>var x=0;</script>";
        string expectedResult = "var x=0";
        string result = Core.Security.SqlInjection.SqlInjection.SecureString(input);

        Assert.AreEqual(expectedResult, result);
    }
}