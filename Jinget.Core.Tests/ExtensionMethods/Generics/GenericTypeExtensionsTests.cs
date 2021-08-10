using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.ExtensionMethods.Generics;
using Jinget.Core.Tests.ExtensionMethods.BaseData;

namespace Jinget.Core.Tests.ExtensionMethods.Generics
{
    [TestClass()]
    public class GenericTypeExtensionsTests
    {
        [TestMethod()]
        public void should_return_true_for_nongeneric_child_nongeneric_parent() => Assert.IsTrue(typeof(NonGenericChildNonGenericParent).IsSubclassOfRawGeneric(typeof(NonGenericParent)));

        [TestMethod()]
        public void should_return_true_for_nongeneric_child_generic_parent() => Assert.IsTrue(typeof(NonGenericChildGenericParent).IsSubclassOfRawGeneric(typeof(GenericParent<>)));

        [TestMethod()]
        public void should_return_true_for_generic_child_nongeneric_parent() => Assert.IsTrue(typeof(GenericChildNonGenericParent<>).IsSubclassOfRawGeneric(typeof(NonGenericParent)));

        [TestMethod()]
        public void should_return_true_for_generic_child_generic_parent() => Assert.IsTrue(typeof(GenericChildGenericParent<>).IsSubclassOfRawGeneric(typeof(GenericParent<>)));

        [TestMethod()]
        public void should_return_true_for_generic_child_multigeneric_parent() => Assert.IsTrue(typeof(GenericChildMultiGenericParent<,,>).IsSubclassOfRawGeneric(typeof(MultiGenericParent<,,>)));

        [TestMethod()]
        public void should_return_true_for_nongeneric_child_multigeneric_parent() => Assert.IsTrue(typeof(NonGenericChildMultiGenericParent).IsSubclassOfRawGeneric(typeof(MultiGenericParent<,,>)));
    }
}
