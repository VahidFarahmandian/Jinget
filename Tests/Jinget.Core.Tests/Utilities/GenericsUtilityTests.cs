namespace Jinget.Core.Tests.Utilities;

[TestClass]
public class GenericsUtilityTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public int? NullableId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // --------------------------------------------------------------------------
    // Tests for DIRECT VALUE COMPARISON (AreEqual<T>(T a, T b))
    // --------------------------------------------------------------------------

    [TestMethod]
    public void AreEqual_DirectValues_ReturnsTrueForEqualValues()
    {
        Assert.IsTrue(GenericsUtility.AreEqual(5, 5));
        Assert.IsTrue(GenericsUtility.AreEqual("test", "test"));
    }

    [TestMethod]
    public void AreEqual_DirectValues_ReturnsFalseForDifferentValues()
    {
        Assert.IsFalse(GenericsUtility.AreEqual(3, 7));
        Assert.IsFalse(GenericsUtility.AreEqual("a", "b"));
    }

    // --------------------------------------------------------------------------
    // Tests for PROPERTY-TO-VALUE COMPARISON (AreEqual<T, TValue>(x => x.Prop, value))
    // --------------------------------------------------------------------------

    [TestMethod]
    public void AreEqual_PropertyAndValue_GeneratesCorrectExpression()
    {
        // Arrange
        var expression = GenericsUtility.AreEqual<TestEntity, int>(x => x.Id, 10);

        // Act
        var compiled = expression.Compile();
        var entity = new TestEntity { Id = 10 };

        // Assert
        Assert.IsTrue(compiled(entity));
        Assert.IsFalse(compiled(new TestEntity { Id = 20 }));
    }

    [TestMethod]
    public void AreEqual_PropertyAndValue_WorksWithNullableTypes()
    {
        int? nullValue = null;
        var expression = GenericsUtility.AreEqual<TestEntity, int?>(x => x.NullableId, nullValue);

        var compiled = expression.Compile();
        Assert.IsTrue(compiled(new TestEntity { NullableId = null }));
        Assert.IsFalse(compiled(new TestEntity { NullableId = 1 }));
    }

    // --------------------------------------------------------------------------
    // Tests for PROPERTY-TO-PROPERTY COMPARISON (AreEqual<T, TValue>(x => x.Prop1, x => x.Prop2))
    // --------------------------------------------------------------------------

    [TestMethod]
    public void AreEqual_TwoProperties_GeneratesCorrectExpression()
    {
        var expression = GenericsUtility.AreEqual<TestEntity, string>(x => x.Name, x => x.Name.ToUpper());

        var compiled = expression.Compile();
        var entity = new TestEntity { Name = "test" };

        Assert.IsFalse(compiled(entity)); // "test" != "TEST"
    }

    // --------------------------------------------------------------------------
    // Tests for VALUE-TO-PROPERTY COMPARISON (AreEqual<T, TValue>(value, x => x.Prop))
    // --------------------------------------------------------------------------

    [TestMethod]
    public void AreEqual_ValueAndProperty_GeneratesCorrectExpression()
    {
        var expression = GenericsUtility.AreEqual<TestEntity, decimal>(
            100.50m,
            x => x.Price);

        var compiled = expression.Compile();

        Assert.IsTrue(compiled(new TestEntity { Price = 100.50m }));
        Assert.IsFalse(compiled(new TestEntity { Price = 200.00m }));
    }

    // --------------------------------------------------------------------------
    // INTEGRATION TEST: Verify EF Core can translate the expression
    // --------------------------------------------------------------------------

    [TestMethod]
    public void AreEqual_PropertyAndValue_IsTranslatableByEFCore()
    {
        // Arrange
        var expression = GenericsUtility.AreEqual<TestEntity, int>(x => x.Id, 5);

        // Act (simulate EF Core query)
        var query = new[] { new TestEntity { Id = 5 }, new TestEntity { Id = 6 } }
            .AsQueryable()
            .Where(expression);

        // Assert
        Assert.AreEqual(1, query.Count());
        Assert.AreEqual(5, query.Single().Id);
    }

    [TestMethod]
    public void AreEqual_TwoProperties_IsTranslatableByEFCore()
    {
        var expression = GenericsUtility.AreEqual<TestEntity, string>(
            x => x.Name,
            x => x.Name.ToUpper());

        var query = new[]
        {
        new TestEntity { Name = "TEST" },
        new TestEntity { Name = "test" }
    }
        .AsQueryable()
        .Where(expression);

        Assert.AreEqual(1, query.Count()); // Only "TEST" matches itself in uppercase
    }
}