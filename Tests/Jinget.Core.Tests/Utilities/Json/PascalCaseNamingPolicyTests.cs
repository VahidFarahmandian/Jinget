namespace Jinget.Core.Tests.Utilities.Json;

[TestClass()]
public class PascalCaseNamingPolicyTests
{
    private PascalCaseNamingPolicy _policy;

    [TestInitialize]
    public void Initialize()
    {
        _policy = new PascalCaseNamingPolicy();
    }

    [TestMethod]
    public void ConvertName_NullOrEmpty_ReturnsSame()
    {
        Assert.IsNull(_policy.ConvertName(null));
        Assert.AreEqual(string.Empty, _policy.ConvertName(string.Empty));
    }

    [TestMethod]
    public void ConvertName_AllUpperCase_ReturnsLowerCase()
    {
        Assert.AreEqual("test", _policy.ConvertName("TEST"));
        Assert.AreEqual("id", _policy.ConvertName("ID"));
    }

    [TestMethod]
    public void ConvertName_FirstCharacterUpperCase_ReturnsFirstCharacterLowerCase()
    {
        Assert.AreEqual("testName", _policy.ConvertName("TestName"));
        Assert.AreEqual("userName", _policy.ConvertName("UserName"));
    }

    [TestMethod]
    public void ConvertName_FirstCharacterLowerCase_ReturnsSame()
    {
        Assert.AreEqual("testName", _policy.ConvertName("testName"));
        Assert.AreEqual("userName", _policy.ConvertName("userName"));
    }

    [TestMethod]
    public void ConvertName_MixedCase_ReturnsFirstCharacterLowerCase()
    {
        Assert.AreEqual("tESTName", _policy.ConvertName("TESTName"));
        Assert.AreEqual("uSERName", _policy.ConvertName("USERName"));
        Assert.AreEqual("tESTName", _policy.ConvertName("tESTName"));
        Assert.AreEqual("uSERName", _policy.ConvertName("uSERName"));

    }

    [TestMethod]
    public void ConvertName_NumbersAndSpecialCharacters_HandlesCorrectly()
    {
        Assert.AreEqual("test123Name", _policy.ConvertName("Test123Name"));
        Assert.AreEqual("user_Name", _policy.ConvertName("User_Name"));
        Assert.AreEqual("user-Name", _policy.ConvertName("User-Name"));
        Assert.AreEqual("123Test", _policy.ConvertName("123Test"));
        Assert.AreEqual("test123", _policy.ConvertName("Test123"));
    }

    [TestMethod]
    public void ConvertName_AlreadyLowerCase_ReturnsSame()
    {
        Assert.AreEqual("test", _policy.ConvertName("test"));
        Assert.AreEqual("name", _policy.ConvertName("name"));
    }

    [TestMethod]
    public void ConvertName_SingleCharacterUpperCase_ReturnsLowerCase()
    {
        Assert.AreEqual("a", _policy.ConvertName("A"));
        Assert.AreEqual("b", _policy.ConvertName("B"));
    }
}