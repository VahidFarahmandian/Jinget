using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Jinget.Extensions.Tests
{
    [TestClass()]
    public class DbTypeExtensionsTests
    {
        [TestMethod()]
        public void Should_return_true_for_numeric_dbType() => Assert.IsTrue(DbType.Int32.IsNumericType());

        [TestMethod()]
        public void Should_return_false_for_non_numeric_dbType() => Assert.IsFalse(DbType.Binary.IsNumericType());

        [TestMethod()]
        public void Should_return_true_for_unicode_dbType_IsUnicodeType() => Assert.IsTrue(DbType.String.IsUnicodeType());

        [TestMethod()]
        public void Should_return_false_for_non_unicode_dbType_IsUnicodeType() => Assert.IsFalse(DbType.AnsiString.IsUnicodeType());

        [TestMethod()]
        public void Should_return_false_for_non_unicode_dbType_IsNonUnicodeType() => Assert.IsFalse(DbType.String.IsNonUnicodeType());

        [TestMethod()]
        public void Should_return_true_for_non_unicode_dbType_IsNonUnicodeType() => Assert.IsTrue(DbType.AnsiString.IsNonUnicodeType());
    }
}
