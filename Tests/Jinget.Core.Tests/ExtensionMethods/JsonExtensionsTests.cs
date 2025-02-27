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
        var result = json.Deserialize<TestType>();

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
        json.Deserialize<InvalidType>(strictPropertyMatching: true);
    }

    [TestMethod]
    public void Deserialize_ValidHierarchicalJson_To_InvalidType_ThrowsException()
    {
        // Arrange
        string json = "{ \"name\": \"Test\", \"Value\": 123, \"Inner1\": { \"isActive\": true, \"Age\": 30, \"Inner2\": { \"IsAlive\": false, \"code\": 987 } } }";

        // Act
        var result = json.Deserialize<TestType>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(123, result.Value);
        Assert.AreEqual(30, result.Inner1.Age);
        Assert.IsTrue(result.Inner1.IsActive);
        Assert.IsFalse(result.Inner1.Inner2.IsAlive);
        Assert.AreEqual(987, result.Inner1.Inner2.Code);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_InvalidJson_ThrowsException()
    {
        // Arrange
        string json = "invalid json";

        // Act
        json.Deserialize<TestType>();
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_NullJson_ThrowsException()
    {
        // Arrange
        string json = null;

        // Act
        json.Deserialize<TestType>();
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_EmptyJson_ThrowsException()
    {
        // Arrange
        string json = "";

        // Act
        json.Deserialize<TestType>();
    }

    [TestMethod]
    public void Serialize_ValidObject_ReturnsJson()
    {
        // Arrange
        var obj = new TestType { Name = "Test", Value = 123 };

        // Act
        var result = obj.Serialize(options: new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        Assert.AreEqual("{\"Name\":\"Test\",\"Value\":123}", result);
    }

    [TestMethod]
    public void Serialize_NullObject_ReturnsEmptyString()
    {
        // Arrange
        TestType obj = null;

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
        var result = json.Deserialize<TestType>(options, strictPropertyMatching: false);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(123, result.Value);
    }

    [TestMethod]
    public void Serialize_ObjectWithOptions_ReturnsJsonWithCustomOptions()
    {
        // Arrange
        var obj = new TestType { Name = "Test", Value = 123 };
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Act
        var result = obj.Serialize(options);

        // Assert
        Assert.AreEqual("{\"name\":\"Test\",\"value\":123,\"inner1\":null}", result);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Deserialize_JsonWithInvalidCharacters_ThrowsException()
    {
        //arrange
        string json = "{\"Name\":\"Test\",Value:123}";

        //act
        json.Deserialize<TestType>();
    }

    [TestMethod]
    public void Serialize_ObjectWithSpecialCharacters_ReturnsJson()
    {
        //arrange
        var obj = new TestType { Name = "Test\"Special", Value = 123 };

        //act
        var result = obj.Serialize();

        //assert
        Assert.AreEqual("{\"Name\":\"Test\\u0022Special\",\"Value\":123,\"Inner1\":null}", result);
    }

}

public class MessageModel
{
    public MessageDetailModel Error { get; set; }
    public class MessageDetailModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
public class TestType
{
    public string Name { get; set; }
    public int Value { get; set; }

    public InnerType1 Inner1 { get; set; }
}
public class InnerType1
{
    public bool IsActive { get; set; }
    public int Age { get; set; }
    public InnerType2 Inner2 { get; set; }
}
public class InnerType2
{
    public bool IsAlive { get; set; }
    public int Code { get; set; }

}
public class InvalidType
{
    public int SomeProperty { get; set; }
}