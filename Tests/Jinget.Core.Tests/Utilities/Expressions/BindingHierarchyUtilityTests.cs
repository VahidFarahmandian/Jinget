namespace Jinget.Core.Tests.Utilities.Expressions;

[TestClass]
public class BindingHierarchyUtilityTests
{
    [TestMethod()]
    public void should_create_bindingexpression_using_bindinghierarchy()
    {
        var prop1 = new BindingHierarchy("Property2", typeof(TestClass));
        List<BindingHierarchy> bindings = [
            prop1,
            new BindingHierarchy("Property3", typeof(TestClass)),
            new BindingHierarchy("InnerProperty1", typeof(InnerClass),new BindingHierarchy("InnerSingularProperty", typeof(TestClass))),
            ];

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            Property2 = x.Property2,
            Property3 = x.Property3,
            InnerSingularProperty = new InnerClass()
            {
                InnerProperty1 = x.InnerSingularProperty.InnerProperty1
            }
        };

        var result = BindingHierarchyUtility.CreateBindingExpression<TestClass>(bindings);

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod()]
    public void should_create_bindingexpression_using_bindinghierarchy_one_many_relation()
    {
        var prop1 = new BindingHierarchy("Property2", typeof(TestClass));
        List<BindingHierarchy> bindings = [
            new BindingHierarchy("InnerProperty2", typeof(InnerClass),new BindingHierarchy("InnerProperty", typeof(TestClass))),
            ];

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            InnerProperty = x.InnerProperty.Select(x => new InnerClass()
            {
                InnerProperty2 = x.InnerProperty2
            }).ToList()
        };

        var result = BindingHierarchyUtility.CreateBindingExpression<TestClass>(bindings);

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_TwoLevel_Collection_Hierarchy()
    {
        // Test binding Id through Parents_1 (collection) -> InnerProperty (collection)
        var bindings = new List<BindingHierarchy>
        {
            new BindingHierarchy("Id", typeof(PublicParentType),
                new BindingHierarchy("Parents_1", typeof(TestClass.InnerClass),
                    new BindingHierarchy("InnerProperty", typeof(TestClass))))
        };

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
        var result = BindingHierarchyUtility.CreateBindingExpression<TestClass>(bindings);
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_Nested_Collection_In_Collection()
    {
        // Test nested collections: Parents_1 -> InnerListProperty -> Parents_1
        var bindings = new List<BindingHierarchy>
        {
            new BindingHierarchy("Id", typeof(PublicParentType),
                new BindingHierarchy("Parents_1", typeof(TestClass.InnerClass),
                    new BindingHierarchy("InnerListProperty", typeof(TestClass))))
        };

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

        var result = BindingHierarchyUtility.CreateBindingExpression<TestClass>(bindings);
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_TwoLevel_Object_Hierarchy()
    {
        // Correct hierarchy: Sub -> Parent_1 (in InnerClass) -> InnerSingularProperty (in TestClass)
        var bindings = new List<BindingHierarchy>
        {
            new BindingHierarchy("Sub",typeof(PublicParentType),
                new BindingHierarchy("Parent_1",typeof(InnerClass),
                    new BindingHierarchy("InnerSingularProperty",typeof(TestClass))))
        };

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

        var result = BindingHierarchyUtility.CreateBindingExpression<TestClass>(bindings);
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_Mixed_Object_And_Collection_Hierarchy()
    {
        // Test complex hierarchy: Property2 -> InnerSingularProperty (object) -> Parents_1 (collection)
        var bindings = new List<BindingHierarchy>
        {
            new BindingHierarchy("Property2", typeof(TestClass)),
            new BindingHierarchy("Id", typeof(PublicParentType),
                new BindingHierarchy("Parents_1", typeof(TestClass.InnerClass),
                    new BindingHierarchy("InnerSingularProperty", typeof(TestClass))))
        };

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

        var result = BindingHierarchyUtility.CreateBindingExpression<TestClass>(bindings);
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_ThreeLevel_Deep_Hierarchy()
    {
        // Test three-level hierarchy: Property3 -> Parent_1 -> InnerListProperty -> Parents_1
        var bindings = new List<BindingHierarchy>
        {
            new BindingHierarchy("Property3", typeof(TestClass)),
            new BindingHierarchy("Sub", typeof(PublicParentType),
                new BindingHierarchy("Parent_1", typeof(TestClass.InnerClass),
                    new BindingHierarchy("InnerListProperty", typeof(TestClass))))
        };

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

        var result = BindingHierarchyUtility.CreateBindingExpression<TestClass>(bindings);
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }
}