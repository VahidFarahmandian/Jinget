namespace Jinget.Core.Tests.ExtensionMethods.Enums;

[TestClass]
public class GenericTypeExtensionsTests
{
    
    [TestMethod]
    public void Should_return_true_for_numeric_dbType() => Assert.IsTrue(DbType.Int32.IsNumericDbType());

    [TestMethod]
    public void Should_return_false_for_non_numeric_dbType() => Assert.IsFalse(DbType.Binary.IsNumericDbType());

    [TestMethod]
    public void Should_return_true_for_unicode_dbType_IsUnicodeType() => Assert.IsTrue(DbType.String.IsUnicodeDbType());

    [TestMethod]
    public void Should_return_false_for_non_unicode_dbType_IsUnicodeType() => Assert.IsFalse(DbType.AnsiString.IsUnicodeDbType());

    [TestMethod]
    public void Should_return_false_for_non_unicode_dbType_IsNonUnicodeType() => Assert.IsFalse(DbType.String.IsNonUnicodeDbType());

    [TestMethod]
    public void Should_return_true_for_non_unicode_dbType_IsNonUnicodeType() => Assert.IsTrue(DbType.AnsiString.IsNonUnicodeDbType());

    [TestMethod]
    public void Should_return_false_for_non_bool_dbType_IsBooleanType() => Assert.IsTrue(DbType.Boolean.IsBooleanDbType());

    [TestMethod]
    public void Should_return_true_for_bool_dbType_IsBooleanType() => Assert.IsTrue(DbType.Boolean.IsBooleanDbType());
}
