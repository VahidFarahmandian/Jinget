namespace Jinget.Core.Tests.ExtensionMethods;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void should_replace_arabic_YeKe_in_string_with_its_farsi_equalivants()
    {
        string input = "Sample text with علي and روشنك";
        string expected = "Sample text with علی and روشنک";

        input = input.ApplyCorrectYeKe();

        Assert.AreEqual(expected, input);
    }

    [TestMethod]
    public void should_return_empty_string()
    {
        Assert.AreEqual(string.Empty, "".ApplyCorrectYeKe());
        Assert.AreEqual(string.Empty, "   ".ApplyCorrectYeKe());
        Assert.AreEqual(string.Empty, string.Empty.ApplyCorrectYeKe());
    }

    [TestMethod]
    public void should_return_CamelCase_string()
    {
        string input = "Vahid Farahmandian";
        string expectedResult = "vahid Farahmandian";
        string result = input.ToCamelCase();
        Assert.AreEqual(expectedResult, result);
    }
}
