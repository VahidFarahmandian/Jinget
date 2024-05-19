using Jinget.Core.Tests._BaseData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace Jinget.Core.Utilities.Enum.Tests
{
    [TestClass()]
    public class DbTypeUtilityTests
    {
        [TestMethod()]
        public void should_return_corresponding_dbtype_based_on_given_type()
        {
            var expectedType = DbType.Int32;
            var result = DbTypeUtility.GetDbType(typeof(int));
            Assert.AreEqual(expectedType, result);
        }

        [TestMethod()]
        public void should_return_corresponding_dbtype_based_on_generic_type()
        {
            var expectedType = DbType.Int32;
            var result = DbTypeUtility.GetDbType<int>();
            Assert.AreEqual(expectedType, result);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void should_raise_exception_for_unknown_dbtype()
        {
            DbTypeUtility.GetDbType<SampleInterfaceClass>();
        }
    }
}