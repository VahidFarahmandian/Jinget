using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.Security.Hashing.Model;
using Jinget.Core.Security.Hashing;

namespace Jinget.Core.Tests.Security.Hashing;

[TestClass()]
public class HashManagerTests
{
    [TestMethod()]
    public void should_compare_given_input_with_its_hashed_value_and_return_true()
    {
        string input = "vahid";
        HashModel hashedResult = new HashManager().Hash(input);

        var result = new HashManager().AreEqual(hashedResult, input);

        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void should_hash_input_with_random_generated_salt()
    {
        string input = "vahid";
        HashModel result = new HashManager().Hash(input);

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Salt));
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.HashedValue));
    }

    [TestMethod()]
    public void should_hash_input_with_specific_salt()
    {
        string input = "vahid";
        string inputsalt = "qsc7Y/0LN/8lKGu409KBRw==";
        string result = new HashManager().Hash(input, inputsalt);

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }
}