using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.ExtensionMethods.Database.SqlClient;
using Jinget.Core.ExtensionMethods;

namespace Jinget.Core.Tests.ExtensionMethods.Database.SqlClient
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod()]
        public void should_replace_arabic_YeKe_in_string_with_its_farsi_equalivants()
        {
            string input = "Sample text with علي and روشنك";
            string expected = "Sample text with علی and روشنک";

            input = input.ApplyCorrectYeKe();

            Assert.AreEqual(expected, input);
        }

        [TestMethod()]
        public void should_return_empty_string()
        {
            Assert.IsTrue("".ApplyCorrectYeKe() == string.Empty);
            Assert.IsTrue("   ".ApplyCorrectYeKe() == string.Empty);
            Assert.IsTrue(string.Empty.ApplyCorrectYeKe() == string.Empty);
        }
    }
}
