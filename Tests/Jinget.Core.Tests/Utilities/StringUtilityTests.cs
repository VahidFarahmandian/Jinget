namespace Jinget.Core.Tests.Utilities;

[TestClass]
public class StringUtilityTests
{
    [TestMethod]
    public void should_create_random_string_using_given_characterset()
    {
        int length = 5;
        IEnumerable<char> charset = new char[] { 'a', 'b', 'c', '1' };

        var result = StringUtility.GetRandomString(length, charset);

        Assert.IsTrue(result.Length == length);
        Assert.IsTrue(result.ToCharArray().All(x => charset.Contains(x)));
    }

    [TestMethod]
    public void should_return_true_for_numeric_string()
    {
        string input = "1234567890";
        Assert.IsTrue(StringUtility.IsDigitOnly(input));
    }

    [TestMethod]
    public void should_return_false_for_non_numeric_string()
    {
        string input = "vahid123";
        Assert.IsFalse(StringUtility.IsDigitOnly(input));
    }

    [TestMethod]
    public void Normalize_NullInput_ReturnsEmptyString()
    {
        string result = StringUtility.Normalize(null);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Normalize_EmptyString_ReturnsEmptyString()
    {
        string result = StringUtility.Normalize(string.Empty);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Normalize_WhitespaceOnly_ReturnsEmptyString()
    {
        string result = StringUtility.Normalize("   \t  \n  ");
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Normalize_SingleWord_ReturnsSameWord()
    {
        string result = StringUtility.Normalize("hello");
        Assert.AreEqual("hello", result);
    }

    [TestMethod]
    public void Normalize_MultipleSpacesBetweenWords_CollapsesToSingleSpace()
    {
        string result = StringUtility.Normalize("hello    world");
        Assert.AreEqual("hello world", result);
    }

    [TestMethod]
    public void Normalize_TabsAndNewlines_CollapsesToSingleSpace()
    {
        string result = StringUtility.Normalize("hello\t\nworld");
        Assert.AreEqual("hello world", result);
    }

    [TestMethod]
    public void Normalize_LeadingAndTrailingSpaces_Removed()
    {
        string result = StringUtility.Normalize("   hello world   ");
        Assert.AreEqual("hello world", result);
    }
}