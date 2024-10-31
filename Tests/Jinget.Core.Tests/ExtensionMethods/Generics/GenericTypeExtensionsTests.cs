﻿namespace Jinget.Core.Tests.ExtensionMethods.Generics;

[TestClass]
public class GenericTypeExtensionsTests
{
    [TestMethod]
    public void should_return_false_for_irrelative_types() => Assert.IsFalse(typeof(System.Tuple).IsSubclassOfRawGeneric(typeof(System.Exception)));

    [TestMethod]
    public void should_return_true_for_nongeneric_child_nongeneric_parent() => Assert.IsTrue(typeof(NonGenericChildNonGenericParent).IsSubclassOfRawGeneric(typeof(NonGenericParent)));

    [TestMethod]
    public void should_return_true_for_nongeneric_child_generic_parent() => Assert.IsTrue(typeof(NonGenericChildGenericParent).IsSubclassOfRawGeneric(typeof(GenericParent<>)));

    [TestMethod]
    public void should_return_true_for_generic_child_nongeneric_parent() => Assert.IsTrue(typeof(GenericChildNonGenericParent<>).IsSubclassOfRawGeneric(typeof(NonGenericParent)));

    [TestMethod]
    public void should_return_true_for_generic_child_generic_parent() => Assert.IsTrue(typeof(GenericChildGenericParent<>).IsSubclassOfRawGeneric(typeof(GenericParent<>)));

    [TestMethod]
    public void should_return_true_for_generic_child_multigeneric_parent() => Assert.IsTrue(typeof(GenericChildMultiGenericParent<,,>).IsSubclassOfRawGeneric(typeof(MultiGenericParent<,,>)));

    [TestMethod]
    public void should_return_true_for_nongeneric_child_multigeneric_parent() => Assert.IsTrue(typeof(NonGenericChildMultiGenericParent).IsSubclassOfRawGeneric(typeof(MultiGenericParent<,,>)));
}
