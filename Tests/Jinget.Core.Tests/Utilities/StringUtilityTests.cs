using Jinget.Core.Tests._BaseData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Jinget.Core.Utilities.Tests;

[TestClass()]
public class StringUtilityTests
{
    [TestMethod()]
    public void should_create_random_string_using_given_characterset()
    {
        int length = 5;
        IEnumerable<char> charset = new char[] { 'a', 'b', 'c', '1' };

        var result = StringUtility.GetRandomString(length, charset);

        Assert.IsTrue(result.Length == length);
        Assert.IsTrue(result.ToCharArray().All(x => charset.Contains(x)));
    }

    [TestMethod()]
    public void should_return_true_for_numeric_string()
    {
        string input = "1234567890";
        Assert.IsTrue(StringUtility.IsDigitOnly(input));
    }

    [TestMethod()]
    public void should_return_false_for_non_numeric_string()
    {
        string input = "vahid123";
        Assert.IsFalse(StringUtility.IsDigitOnly(input));
    }
}