using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.Tests._BaseData;

namespace Jinget.Core.Operators.Tests;

[TestClass()]
public class GenericEqualityOperatorTests
{
    class Test<T> where T : struct
    {
        public bool AreEqual() => GenericEqualityOperator<T>.AreEqual(new SampleGeneric<T>().Id, default);
    }
    [TestMethod()]
    public void should_return_true_for_same_generic_values()
    {
        bool result = new Test<int>().AreEqual();
        Assert.IsTrue(result);
    }
}