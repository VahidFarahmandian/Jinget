using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinget.Core.Security.SqlInjection.Tests
{
    [TestClass()]
    public class SqlInjectionTests
    {
        [TestMethod()]
        public void should_immunize_string()
        {
            string input = "<script>var x=0;</script>";
            string expectedResult = ">var x=0</";
            string result = SqlInjection.SecureString(input);

            Assert.AreEqual(expectedResult, result);
        }
    }
}