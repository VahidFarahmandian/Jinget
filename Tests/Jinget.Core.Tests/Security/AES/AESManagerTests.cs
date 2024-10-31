using Jinget.Core.Security.AES;

namespace Jinget.Core.Tests.Security.AES;

[TestClass]
public class AESManagerTests
{
    [TestMethod]
    public void should_encrypt_the_string_using_given_key_and_iv()
    {
        string input = "vahid";
        string key = "12345678901234567890123456789012";
        string iv = "1234567890123456";
        string expectedResult = "sQyEKQhH++6olei3SXbWag==";
        var result = AESManager.Encrypt(input, key, iv);

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void should_decrypt_the_string_using_given_key_and_iv()
    {
        string encryptedInput = "sQyEKQhH++6olei3SXbWag==";
        string key = "12345678901234567890123456789012";
        string iv = "1234567890123456";
        string expectedResult = "vahid";
        var result = AESManager.Decrypt(encryptedInput, key, iv);
        Assert.AreEqual(expectedResult, result);
    }
}