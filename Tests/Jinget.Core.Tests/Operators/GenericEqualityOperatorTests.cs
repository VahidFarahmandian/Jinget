using Jinget.Core.Operators;

namespace Jinget.Core.Tests.Operators;

[TestClass]
public class GenericEqualityOperatorTests
{
    class Test<T> where T : struct
    {
        public bool AreEqual() => GenericEqualityOperator<T>.AreEqual(new SampleGeneric<T>().Id, default);
    }
    [TestMethod]
    public void should_return_true_for_same_generic_values()
    {
        bool result = new Test<int>().AreEqual();
        Assert.IsTrue(result);
    }
}