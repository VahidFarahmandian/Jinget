using Jinget.Core.Contracts;
using static Jinget.Core.Utilities.Expressions.BindingHierarchyUtility.BindingHierarchyApi;
namespace Jinget.Core.Tests.Utilities.Expressions.BindingHierarchyUtility;

[TestClass]
public class BindingHierarchyUtilityTests
{
    [TestMethod()]
    public void should_create_bindingexpression_using_bindinghierarchy()
    {
        List<BindingDefinition> list = [
            Property<TestClass>("Property2"),
            Property<InnerClass>("InnerProperty1").WithParent(Property<TestClass>("InnerSingularProperty"))
            ];
        list.Add(Property<TestClass>("Property3"));
        var bindings = Define<TestClass>(
            [.. list]
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            Property2 = x.Property2,
            InnerSingularProperty = new InnerClass()
            {
                InnerProperty1 = x.InnerSingularProperty.InnerProperty1
            },
            Property3 = x.Property3
        };

        var result = bindings.Compile();

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod()]
    public void should_create_bindingexpression_using_bindinghierarchy_and_type()
    {
        var bindings = Define<TestClass>(
            Property("Property2", typeof(TestClass)),
            Property<InnerClass>("InnerProperty1").WithParent(Property("InnerSingularProperty", typeof(TestClass)))
            );

        Expression<Func<TestClass, TestClass>> expectedResult = x => new TestClass()
        {
            Property2 = x.Property2,
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
            InnerProperty = x.InnerProperty.Select(x => new InnerClass()
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
            InnerListProperty = x.InnerListProperty.Select(x => new InnerClass()
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
            InnerSingularProperty = new InnerClass()
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
            InnerSingularProperty = new InnerClass()
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
            InnerListProperty = x.InnerListProperty.Select(x => new InnerClass()
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
    [TestMethod]
    public void Should_Create_Binding_For_Jinget_Model()
    {
        var result = new CustomerModel().GetConstantFields();

        Expression<Func<CustomerModel, CustomerModel>> expectedResult = x => new CustomerModel()
        {
            Trace = new CustomTrace()
            {
                InsertDate = x.Trace.InsertDate,
                CreatedBy = x.Trace.CreatedBy
            },
            Name = x.Name
        };

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }
}

public abstract class BaseTraceData<TUserContext>
{
    public virtual Expression<Func<TModelType, TModelType>> GetConstantFieldsExpression<TModelType>(
        List<BindingDefinition>? customBindings = null,
        string tracePropertyName = "Trace")
    {
        return Define<TModelType>(GetConstantFields<TModelType>(customBindings, tracePropertyName)).Compile();
    }
    public virtual BindingDefinition[] GetConstantFields<TModelType>(
        List<BindingDefinition>? customBindings = null,
        string tracePropertyName = "Trace")
    {
        List<BindingDefinition> bindingsList = [
            Property(nameof(InsertDate), GetType()).WithParent(Property<TModelType>(tracePropertyName)),
            Property(nameof(CreatedBy), GetType()).WithParent(Property<TModelType>(tracePropertyName))
            ];
        if (customBindings != null && customBindings.Count != 0)
            bindingsList.AddRange(customBindings);

        return bindingsList.ToArray();
    }

    public string? CreatedBy { get; set; }

    public DateTime InsertDate { get; set; } = DateTime.UtcNow;
}
public abstract class TraceBaseEntity<TTraceDataType, TUserContext, TKeyType> : BaseEntity<TKeyType>
    where TTraceDataType : BaseTraceData<TUserContext>
    where TUserContext : BaseUserContext
{
    protected TraceBaseEntity() { }
    protected TraceBaseEntity(TKeyType id) : base(id) { }

    public abstract TTraceDataType Trace { get; set; }
}
public class CustomerModel : TraceBaseEntity<CustomTrace, CustomUserContext, int>, IAggregateRoot
{
    public string? Name { get; set; }

    public override CustomTrace Trace { get; set; } = new();

    public Expression<Func<CustomerModel, CustomerModel>> GetConstantFields()
    {
        var bindings = new CustomTrace().GetConstantFields<CustomerModel>([Property<CustomerModel>(nameof(Name))]);
        var definition = Define<CustomerModel>(bindings);
        return definition.Compile();
    }
}
public class CustomTrace : BaseTraceData<CustomUserContext>
{
    public override Expression<Func<TModelType, TModelType>> GetConstantFieldsExpression<TModelType>(List<BindingDefinition>? customBindings = null, string tracePropertyName = "Trace")
    {
        return base.GetConstantFieldsExpression<TModelType>(customBindings, tracePropertyName);
    }
}

public class CustomUserContext : BaseUserContext { }
public abstract class BaseUserContext { }
public abstract class BaseEntity<TKeyType> : IEntity
{
    protected BaseEntity() { }

    protected BaseEntity(TKeyType id) : this() => this.SetId(id);

    private TKeyType _id;

    public virtual TKeyType Id
    {
        get => _id;
        protected set
        {
            if (value != null)
            {
                _id = value;
            }
        }
    }
    public virtual void SetId(TKeyType id) => Id = id;
}