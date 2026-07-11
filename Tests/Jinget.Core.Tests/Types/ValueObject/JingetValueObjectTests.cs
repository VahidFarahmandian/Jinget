using Jinget.Core.Types.ValueObject;

namespace Jinget.Core.Tests.Types.ValueObject;

[TestClass]
public class JingetValueObjectTests
{
    #region Equals

    [TestMethod]
    public void Equals_SameReference_ReturnsTrue()
    {
        var value = new TestValueObject("Ali", 30, "A");

        Assert.IsTrue(value.Equals(value));
    }

    [TestMethod]
    public void Equals_Null_ReturnsFalse()
    {
        var value = new TestValueObject("Ali", 30);

        Assert.IsFalse(value.Equals(null));
    }

    [TestMethod]
    public void Equals_DifferentType_ReturnsFalse()
    {
        JingetValueObject left = new TestValueObject("Ali", 30);
        JingetValueObject right = new AnotherValueObject("Ali");

        Assert.IsFalse(left.Equals(right));
    }

    [TestMethod]
    public void Equals_SameValues_ReturnsTrue()
    {
        var left = new TestValueObject("Ali", 30, "GET", "POST");
        var right = new TestValueObject("Ali", 30, "GET", "POST");

        Assert.IsTrue(left.Equals(right));
    }

    [TestMethod]
    public void Equals_DifferentString_ReturnsFalse()
    {
        var left = new TestValueObject("Ali", 30);
        var right = new TestValueObject("Reza", 30);

        Assert.IsFalse(left.Equals(right));
    }

    [TestMethod]
    public void Equals_DifferentInt_ReturnsFalse()
    {
        var left = new TestValueObject("Ali", 30);
        var right = new TestValueObject("Ali", 31);

        Assert.IsFalse(left.Equals(right));
    }

    [TestMethod]
    public void Equals_StringComparedByValue()
    {
        var left = new TestValueObject("ABC", 1);
        var right = new TestValueObject(new string("ABC".ToCharArray()), 1);

        Assert.IsTrue(left.Equals(right));
    }

    [TestMethod]
    public void Equals_CollectionsWithSameValues_ReturnsTrue()
    {
        var left = new TestValueObject("Ali", 30, "GET", "POST");
        var right = new TestValueObject("Ali", 30, "GET", "POST");

        Assert.IsTrue(left.Equals(right));
    }

    [TestMethod]
    public void Equals_CollectionsDifferentOrder_ReturnsFalse()
    {
        var left = new TestValueObject("Ali", 30, "GET", "POST");
        var right = new TestValueObject("Ali", 30, "POST", "GET");

        Assert.IsFalse(left.Equals(right));
    }

    [TestMethod]
    public void Equals_CollectionsDifferentCount_ReturnsFalse()
    {
        var left = new TestValueObject("Ali", 30, "GET");
        var right = new TestValueObject("Ali", 30, "GET", "POST");

        Assert.IsFalse(left.Equals(right));
    }

    [TestMethod]
    public void Equals_CollectionReferenceDifferent_ContentSame_ReturnsTrue()
    {
        var tags1 = new List<string> { "A", "B", "C" };
        var tags2 = new List<string> { "A", "B", "C" };

        var left = new TestValueObject("Ali", 30, tags1.ToArray());
        var right = new TestValueObject("Ali", 30, tags2.ToArray());

        Assert.IsTrue(left.Equals(right));
    }

    [TestMethod]
    public void Equals_NullProperties_ReturnTrue()
    {
        var left = new NullableValueObject(null);
        var right = new NullableValueObject(null);

        Assert.IsTrue(left.Equals(right));
    }

    [TestMethod]
    public void Equals_NullCollections_ReturnTrue()
    {
        var left = new NullableCollectionValueObject(null);
        var right = new NullableCollectionValueObject(null);

        Assert.IsTrue(left.Equals(right));
    }

    [TestMethod]
    public void Equals_NestedValueObjects_ReturnTrue()
    {
        var left = new PersonValueObject(new AddressValueObject("Tehran"));
        var right = new PersonValueObject(new AddressValueObject("Tehran"));

        Assert.IsTrue(left.Equals(right));
    }

    #endregion

    #region Equality Contract

    [TestMethod]
    public void Equals_IsReflexive()
    {
        var value = new TestValueObject("Ali", 30);

        Assert.IsTrue(value.Equals(value));
    }

    [TestMethod]
    public void Equals_IsSymmetric()
    {
        var left = new TestValueObject("Ali", 30);
        var right = new TestValueObject("Ali", 30);

        Assert.IsTrue(left.Equals(right));
        Assert.IsTrue(right.Equals(left));
    }

    [TestMethod]
    public void Equals_IsTransitive()
    {
        var a = new TestValueObject("Ali", 30);
        var b = new TestValueObject("Ali", 30);
        var c = new TestValueObject("Ali", 30);

        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(b.Equals(c));
        Assert.IsTrue(a.Equals(c));
    }

    [TestMethod]
    public void Equals_IsConsistent()
    {
        var left = new TestValueObject("Ali", 30);
        var right = new TestValueObject("Ali", 30);

        for (var i = 0; i < 100; i++)
            Assert.IsTrue(left.Equals(right));
    }

    #endregion

    #region HashCode

    [TestMethod]
    public void GetHashCode_EqualObjects_AreEqual()
    {
        var left = new TestValueObject("Ali", 30, "A", "B");
        var right = new TestValueObject("Ali", 30, "A", "B");

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_IsStable()
    {
        var value = new TestValueObject("Ali", 30);

        var hash = value.GetHashCode();

        for (var i = 0; i < 100; i++)
            Assert.AreEqual(hash, value.GetHashCode());
    }

    #endregion

    #region Operators

    [TestMethod]
    public void OperatorEquals_ReturnsTrue()
    {
        var left = new TestValueObject("Ali", 30);
        var right = new TestValueObject("Ali", 30);

        Assert.IsTrue(left == right);
    }

    [TestMethod]
    public void OperatorNotEquals_ReturnsTrue()
    {
        var left = new TestValueObject("Ali", 30);
        var right = new TestValueObject("Reza", 30);

        Assert.IsTrue(left != right);
    }

    [TestMethod]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        TestValueObject? left = null;
        TestValueObject? right = null;

        Assert.IsTrue(left == right);
    }

    [TestMethod]
    public void OperatorEquals_LeftNull_ReturnsFalse()
    {
        TestValueObject? left = null;
        var right = new TestValueObject("Ali", 30);

        Assert.IsFalse(left == right);
    }

    [TestMethod]
    public void OperatorEquals_RightNull_ReturnsFalse()
    {
        var left = new TestValueObject("Ali", 30);
        TestValueObject? right = null;

        Assert.IsFalse(left == right);
    }

    [TestMethod]
    public void Operators_AgreeWithEquals()
    {
        var left = new TestValueObject("Ali", 30);
        var right = new TestValueObject("Ali", 30);

        Assert.AreEqual(left.Equals(right), left == right);
    }

    #endregion
}