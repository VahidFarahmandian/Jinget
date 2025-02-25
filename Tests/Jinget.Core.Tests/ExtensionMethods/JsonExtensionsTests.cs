using System.Text.Json;

namespace Jinget.Core.Tests.ExtensionMethods;

[TestClass]
public class JsonExtensionsTests
{
    [TestMethod]
    public void Deserialize_ValidJson_ReturnsObject()
    {
        // Arrange
        string json = "{\"Name\":\"Test\",\"Value\":123}";

        // Act
        var result = json.Deserialize<TestObject>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(123, result.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_ValidJson_To_InvalidType_ThrowsException()
    {
        // Arrange
        string json = "{\"Name\":\"Test\",\"Value\":123}";

        // Act
        json.Deserialize<InvalidType>();
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_InvalidJson_ThrowsException()
    {
        // Arrange
        string json = "invalid json";

        // Act
        json.Deserialize<TestObject>();
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_NullJson_ThrowsException()
    {
        // Arrange
        string json = null;

        // Act
        json.Deserialize<TestObject>();
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_EmptyJson_ThrowsException()
    {
        // Arrange
        string json = "";

        // Act
        json.Deserialize<TestObject>();
    }

    [TestMethod]
    public void Serialize_ValidObject_ReturnsJson()
    {
        // Arrange
        var obj = new TestObject { Name = "Test", Value = 123 };

        // Act
        var result = obj.Serialize();

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        Assert.AreEqual("{\"Name\":\"Test\",\"Value\":123}", result);
    }

    [TestMethod]
    public void Serialize_NullObject_ReturnsEmptyString()
    {
        // Arrange
        TestObject obj = null;

        // Act
        var result = obj.Serialize();

        // Assert
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void Deserialize_JsonWithOptions_ReturnsObjectWithCustomOptions()
    {
        // Arrange
        string json = "{\"NAME\":\"Test\",\"VALUE\":123}";
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        var result = json.Deserialize<TestObject>(options, strictPropertyMatching: false);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(123, result.Value);
    }

    [TestMethod]
    public void Serialize_ObjectWithOptions_ReturnsJsonWithCustomOptions()
    {
        // Arrange
        var obj = new TestObject { Name = "Test", Value = 123 };
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        // Act
        var result = obj.Serialize(options);

        // Assert
        Assert.AreEqual("{\"name\":\"Test\",\"value\":123}", result);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_JsonWithInvalidCharacters_ThrowsException()
    {
        //arrange
        string json = "{\"Name\":\"Test\",Value:123}";

        //act
        json.Deserialize<TestObject>();
    }

    [TestMethod]
    public void Serialize_ObjectWithSpecialCharacters_ReturnsJson()
    {
        //arrange
        var obj = new TestObject { Name = "Test\"Special", Value = 123 };

        //act
        var result = obj.Serialize();

        //assert
        Assert.AreEqual("{\"Name\":\"Test\\u0022Special\",\"Value\":123}", result);
    }

}

public class TestObject
{
    public string Name { get; set; }
    public int Value { get; set; }
}
public class InvalidType
{
    public int SomeProperty { get; set; }
}