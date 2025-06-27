using static Jinget.Core.Utilities.Expressions.BindingHierarchyExtensions.BindingHierarchyApi;
namespace Jinget.Core.Tests.Utilities.Expressions;

[TestClass]
public class BindingHierarchyUtilityTests
{
    [TestMethod()]
    public void should_create_bindingexpression_using_bindinghierarchy()
    {
        var bindings = Define<TestClass>(
            Property<TestClass>("Property2"),
            Property<TestClass>("Property3"),
            Property<InnerClass>("InnerProperty1").WithParent(Property<TestClass>("InnerSingularProperty"))
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            Property2 = x.Property2,
            Property3 = x.Property3,
            InnerSingularProperty = new InnerClass()
            {
                InnerProperty1 = x.InnerSingularProperty.InnerProperty1
            }
        };

        var result = bindings.Compile();

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod()]
    public void should_create_bindingexpression_using_bindinghierarchy_one_many_relation()
    {
        var bindings = Define<TestClass>(
            Property<TestClass>("Property2"),
            Property<InnerClass>("InnerProperty2")
                .WithParent(Property<TestClass>("InnerProperty"))
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            Property2 = x.Property2,
            InnerProperty = x.InnerProperty.Select(x => new InnerClass()
            {
                InnerProperty2 = x.InnerProperty2
            }).ToList()
        };

        var result = bindings.Compile();

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_TwoLevel_Collection_Hierarchy()
    {
        var bindings = Define<TestClass>(
            Property<PublicParentType>("Id")
            .WithParent(
                Property<InnerClass>("Parents_1")
                    .WithParent(Property<TestClass>("InnerProperty"))
                   )
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            InnerProperty = x.InnerProperty.Select(x => new TestClass.InnerClass()
            {
                Parents_1 = x.Parents_1.Select(y => new PublicParentType()
                {
                    Id = y.Id
                }).ToList()
            }).ToList()
        };
        var result = bindings.Compile();
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_Nested_Collection_In_Collection()
    {
        var bindings = Define<TestClass>(
            Property<PublicParentType>("Id")
            .WithParent(
                Property<InnerClass>("Parents_1")
                .WithParent(Property<TestClass>("InnerListProperty"))
                )
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            InnerListProperty = x.InnerListProperty.Select(x => new TestClass.InnerClass()
            {
                Parents_1 = x.Parents_1.Select(y => new PublicParentType()
                {
                    Id = y.Id
                }).ToList()
            }).ToList()
        };

        var result = bindings.Compile();
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_TwoLevel_Object_Hierarchy()
    {
        var bindings = Define<TestClass>(
            Property<PublicParentType>("Sub")
            .WithParent(
                Property<InnerClass>("Parent_1")
                .WithParent(Property<TestClass>("InnerSingularProperty"))
                )
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            InnerSingularProperty = new TestClass.InnerClass()
            {
                Parent_1 = new PublicParentType()
                {
                    Sub = x.InnerSingularProperty.Parent_1.Sub
                }
            }
        };

        var result = bindings.Compile();
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_Mixed_Object_And_Collection_Hierarchy()
    {
        var bindings = Define<TestClass>(
            Property<TestClass>("Property2"),
            Property<PublicParentType>("Id")
            .WithParent(
                Property<InnerClass>("Parents_1")
                .WithParent(Property<TestClass>("InnerSingularProperty"))
                )
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            Property2 = x.Property2,
            InnerSingularProperty = new TestClass.InnerClass()
            {
                Parents_1 = x.InnerSingularProperty.Parents_1.Select(x => new PublicParentType()
                {
                    Id = x.Id
                }).ToList()
            }
        };

        var result = bindings.Compile();
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_ThreeLevel_Deep_Hierarchy()
    {
        var bindings = Define<TestClass>(
            Property<TestClass>("Property3"),
            Property<PublicParentType>("Sub")
            .WithParent(
                Property<InnerClass>("Parent_1")
                .WithParent(Property<TestClass>("InnerListProperty"))
                )
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            Property3 = x.Property3,
            InnerListProperty = x.InnerListProperty.Select(x => new TestClass.InnerClass()
            {
                Parent_1 = new PublicParentType()
                {
                    Sub = x.Parent_1.Sub
                }
            }).ToList()
        };

        var result = bindings.Compile();
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }
}