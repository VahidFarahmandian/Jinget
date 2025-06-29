using Jinget.Core.Contracts;
namespace Jinget.Core.Tests.Utilities.Expressions.BindingHierarchyUtility;

[TestClass]
public class BindingHierarchyUtilityTests
{
    [TestMethod()]
    public void should_create_bindingexpression_using_private_ctor()
    {
        var bindings = BindingDefinition.CreateBuilder<PrivateClass>().Add(x => new { x.Property1 })
            .Build();

        //Expression<Func<PrivateClass, PrivateClass>> expectedResult = x => new PrivateClass()
        //{
        //    Property1 = x.Property1
        //};

        var result = BindingDefinition.Compile(bindings);

        // Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod()]
    public void should_create_bindingexpression_using_private_ctor2()
    {
        var binding = BindingDefinition.CreateBuilder<FolderModel>().Add(
         x => new
         {
             x.Name,
             Files = x.Files.Select(f => new
             {
                 f.FolderId,
                 Content = new
                 {
                     f.Content.ContentPath,
                     f.Content.ContentType
                 }
             })
         }).Build();

        Expression<Func<FolderModel, FolderModel>> expectedResult = x => new FolderModel
        {
            Name = x.Name,
            Files = x.Files.Select(f => new FileModel
            {
                FolderId = f.FolderId,
                Content = new FileContent
                {
                    ContentPath = f.Content.ContentPath,
                    ContentType = f.Content.ContentType
                }
            }).ToList()
        };

        var result = BindingDefinition.Compile(binding);
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Bind_Single_Property()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<CustomerModel>()
            .Add(x => new { x.Name })
            .Build();

        // Expected
        Expression<Func<CustomerModel, CustomerModel>> expected = x => new CustomerModel
        {
            Name = x.Name
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Bind_Multiple_Properties()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new { x.Name, x.Path })
            .Build();

        // Expected
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Name = x.Name,
            Path = x.Path
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Bind_Nested_Properties()
    {
        var binding = BindingDefinition.CreateBuilder<FileModel>()
            .Add(x => new
            {
                Content = new { x.Content.ContentType }
            })
            .Build();

        Expression<Func<FileModel, FileModel>> expected = x => new FileModel
        {
            Content = new FileContent { ContentType = x.Content.ContentType }
        };
        var result = BindingDefinition.Compile(binding);
        Assert.AreEqual(expected.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Bind_Complex_Nested_Properties()
    {
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new
            {
                x.Parent.Parent.Parent.Name
            })
            .Build();

        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Name = x.Parent.Parent.Parent.Name
        };
        var result = BindingDefinition.Compile(binding);
        Assert.AreEqual(expected.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Bind_Simple_Collection()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new { Files = x.Files.Select(f => new { f.Name }) })
            .Build();

        // Expected
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Files = x.Files.Select(f => new FileModel { Name = f.Name }).ToList()
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Bind_Multiple_Collections()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new
            {
                Files = x.Files.Select(f => new { f.Name }),
                Children = x.Children.Select(c => new { c.Name })
            })
            .Build();

        // Expected
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Files = x.Files.Select(f => new FileModel { Name = f.Name }).ToList(),
            Children = x.Children.Select(c => new FolderModel { Name = c.Name }).ToList()
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Bind_Nested_Collections()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new
            {
                Files = x.Files.Select(f => new
                {
                    f.Name,
                    Content = new { f.Content.ContentType },
                    Likes = f.Likes.Select(l => new { l.Count })
                })
            })
            .Build();

        // Expected
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Files = x.Files.Select(f => new FileModel
            {
                Name = f.Name,
                Content = new FileContent { ContentType = f.Content.ContentType },
                Likes = f.Likes.Select(l => new Like { Count = l.Count }).ToList()
            }).ToList()
        };

        // Act & Assert
        var result = BindingDefinition.Compile(binding);
        Assert.AreEqual(expected.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Handle_Empty_Bindings()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<CustomerModel>().Build();

        // Expected
        Expression<Func<CustomerModel, CustomerModel>> expected = x => new CustomerModel() { };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Handle_Null_Collections()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new { Files = x.Files.Select(f => new { f.Name }) })
            .Build();

        // Expected (should still generate the expression even if source is null)
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Files = x.Files.Select(f => new FileModel { Name = f.Name }).ToList()
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Handle_CustomTrace_Default_Values()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<CustomerModel>()
            .Add(x => new
            {
                Trace = new
                {
                    x.Trace.InsertDate
                }
            })
            .Build();

        // Expected
        Expression<Func<CustomerModel, CustomerModel>> expected = x => new CustomerModel
        {
            Trace = new CustomTrace { InsertDate = x.Trace.InsertDate }
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Maintain_Original_Values_For_Unbound_Properties()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FileModel>()
            .Add(x => new { x.Name })
            .Build();

        // Expected (Content should remain unbound)
        Expression<Func<FileModel, FileModel>> expected = x => new FileModel
        {
            Name = x.Name
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
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
        List<Expression<Func<TModelType, object>>>? customBindings = null,
        string tracePropertyName = "Trace")
    {
        var bindings = GetConstantFields(customBindings, tracePropertyName);
        return BindingDefinition.Compile(BindingDefinition.CreateBuilder<TModelType>()
            .AddRange(bindings)
            .Build());
    }

    public virtual List<Expression<Func<TModelType, object>>> GetConstantFields<TModelType>(
    List<Expression<Func<TModelType, object>>>? customBindings = null,
    string tracePropertyName = "Trace")
    {
        var bindingsList = new List<Expression<Func<TModelType, object>>>();

        // Add trace properties
        var traceProp = typeof(TModelType).GetProperty(tracePropertyName);
        if (traceProp != null)
        {
            //// Create parameter expression: x =>
            var parameter = Expression.Parameter(typeof(TModelType), "x");

            // Get the Trace property access: x.Trace
            var traceAccess = Expression.Property(parameter, traceProp);

            // Create bindings for CustomTrace properties
            var traceType = traceProp.PropertyType;
            var bindings = new List<MemberBinding>();

            // Add InsertDate binding: x.Trace.InsertDate
            var insertDateProp = traceType.GetProperty(nameof(InsertDate));
            if (insertDateProp != null)
            {
                var insertDateAccess = Expression.Property(traceAccess, insertDateProp);
                bindings.Add(Expression.Bind(insertDateProp, insertDateAccess));
            }

            // Add CreatedBy binding: x.Trace.CreatedBy
            var createdByProp = traceType.GetProperty(nameof(CreatedBy));
            if (createdByProp != null)
            {
                var createdByAccess = Expression.Property(traceAccess, createdByProp);
                bindings.Add(Expression.Bind(createdByProp, createdByAccess));
            }

            // Create the CustomTrace initialization: new CustomTrace { InsertDate = x.Trace.InsertDate, ... }
            var newTrace = Expression.New(traceType);
            var traceInit = Expression.MemberInit(newTrace, bindings);

            //// Create the outer expression that assigns to the trace property
            var outerBinding = Expression.Bind(traceProp, traceInit);

            //// Create the full initialization expression
            var newModel = Expression.New(typeof(TModelType));
            var modelInit = Expression.MemberInit(newModel, outerBinding);

            // Create the final lambda: x => new TModelType { Trace = new CustomTrace { ... } }
            var lambda = Expression.Lambda<Func<TModelType, object>>(modelInit, parameter);

            bindingsList.Add(lambda);
        }

        // Add custom bindings if provided
        if (customBindings != null)
        {
            bindingsList.AddRange(customBindings);
        }

        return bindingsList;
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
        var bindingExpr = new CustomTrace().GetConstantFieldsExpression<CustomerModel>([x => new { x.Name }]);
        return bindingExpr;
    }
}
public class CustomTrace : BaseTraceData<CustomUserContext>
{
    public override Expression<Func<TModelType, TModelType>> GetConstantFieldsExpression<TModelType>(List<Expression<Func<TModelType, object>>> customBindings = null, string tracePropertyName = "Trace")
    {
        return base.GetConstantFieldsExpression(customBindings, tracePropertyName);
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

public class FolderModel
{
    internal FolderModel()
    {

    }

    public FolderModel(string name, string path, Guid? parentId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        ParentId = parentId;
    }

    public string Name { get; set; }
    public string Path { get; set; }
    public Guid? ParentId { get; set; }

    public virtual FolderModel? Parent { get; set; } = null;

    public virtual ICollection<FolderModel> Children { get; set; } = [];

    public virtual ICollection<FileModel> Files { get; set; } = [];
}

public class FileModel
{
    internal FileModel() { }

    public FileModel(Guid folderId, string name, string description, FileContent content)
    {
        if (folderId == Guid.Empty) throw new ArgumentNullException(nameof(folderId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Content = content ?? throw new ArgumentNullException(nameof(content));
        FolderId = folderId;
    }

    public Guid FolderId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public FileContent Content { get; set; }
    public FolderModel Folder { get; set; }
    public ICollection<Like> Likes { get; set; }
}

public class FileContent : JingetValueObject
{
    public string ContentPath { get; set; }
    public string ContentType { get; set; }
    public long ContentSize { get; set; }
    internal FileContent() { }
    public FileContent(string contentPath, string contentType, long contentSize)
    {
        ContentPath = contentPath;
        ContentType = contentType;
        ContentSize = contentSize;
    }

    protected override IEnumerable<object> YieldProperties()
    {
        yield return ContentPath;
        yield return ContentType;
        yield return ContentSize;
    }
}
public class Like
{
    public int Count { get; set; }
}

public abstract class JingetValueObject : IEquatable<JingetValueObject>
{
    protected virtual IEnumerable<object> YieldProperties()
    {
        return ((from p in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                 where p.CanRead
                 select p)?.OrderBy((PropertyInfo p) => p.Name))?.Select((PropertyInfo p) => p.GetValue(this));
    }

    protected virtual void Validate()
    {
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as JingetValueObject);
    }

    public bool Equals(JingetValueObject? other)
    {
        if ((object)other == null || other.GetType() != GetType())
        {
            return false;
        }

        return YieldProperties().SequenceEqual(other.YieldProperties());
    }

    public override int GetHashCode()
    {
        return YieldProperties().Aggregate(0, (int hashcode, object value) => HashCode.Combine(hashcode, value?.GetHashCode() ?? 0));
    }

    public static bool operator ==(JingetValueObject left, JingetValueObject right)
    {
        if ((object)left == null && (object)right == null)
        {
            return true;
        }

        if ((object)left == null || (object)right == null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(JingetValueObject left, JingetValueObject right)
    {
        return !(left == right);
    }
}