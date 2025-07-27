# Jinget

**We are currently in the way to make Jinget an open source project, during this journey we will publish different parts of Jinget**

## Jinget.SourceGenerator
The purpose of this package is to facilitate communication and use of various types of web services and Web APIs including Rest APIs and SOAP web services.

### How to Use:

1.  Download the package from NuGet using Package Manager:
`Install-Package Jinget.SourceGenerator`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.SourceGenerator "Here") for more information.

#### ReadOnly model generation:
While working with CQRS you might need to sperate the Models and ReadOnly Models, where Models are needed for command operations and ReadOnly Models are needed for query operations. 
Consider we have Model as below:

```csharp
public class StudentModel : BaseEntity<int>, IAggregateRoot
{
    public string Name { get; private set; }
    public DateTime EnrollDate { get; set; }
    public Address HomeAddress { get; set; }
    public ICollection<CourseModel> Courses { get; set; }
    public ICollection<StudentScoreModel> Scores { get; set; }
}
```

to generate ReadOnly Model we can add following attributes:

```csharp
namespace Models;
[GenerateReadModel(PreserveBaseTypes = true)]
[AppendPropertyToReadModel("bool", "IsSuspended")]
public class StudentModel : BaseEntity<int>, IAggregateRoot
{
    [PreserveOriginalGetterSetter]
    public string Name { get; private set; }

    [IgnoreReadModelConversion]
    public DateTime EnrollDate { get; set; }

    [PreserveOriginalType]
    public Address HomeAddress { get; set; }

    [Count("CoursesCount")]
    public ICollection<CourseModel> Courses { get; set; }

    [Sum(generatedPropertyName: "SumOfScores")]
    [Average(generatedPropertyName: "AverageScores")]
    public ICollection<StudentScoreModel> Scores { get; set; }
}

```

`GenerateReadModel`: This is the main attribute should be added to model, which cause the readonly model generation. Important note here is that, models should have `Model` suffix. The `PreserveBaseTypes` argument states that if the base type should be ignored for transformation or not.

`AppendPropertyToReadModel`: This is an optional attribute which is used to add new custom property to readonly model. First argument is property type and second argument is it's name.

`PreserveOriginalGetterSetter`: This is an optional attribute which is used to preserve property's getter/setter access modifier. Properties without this attribute will be transformed as public get/set properties.

`IgnoreReadModelConversion`: This is an optional attribute which is used to ignore a property from transformation.

`PreserveOriginalType`: This is an optional attribute which is used to preserve property's original type while transformation. By default all reference types are included in transformation.

`Count`: Just same as `AppendPropertyToReadModel` this attribute is used to add new custom property to readonly model. Other aggregation such as `Sum`, `Average`, `Max` and `Min` are also supported.

Finally above mentioned code will produced the following readonly model:

```csharp
using Jinget.Domain.Core.Entities;
using Jinget.Core.Attributes.AggregationAttributes;
namespace Models;

public class ReadOnlyStudentModel : BaseEntity<int>, Jinget.Core.Contracts.IAggregateRoot
{
    public string Name { public get; private set }
	public Address HomeAddress { get; set; }
	public ICollection<ReadOnlyCourseModel> Courses { get; set; }
	public int CoursesCount { get; set; }
	public ICollection<ReadOnlyStudentScoreModel> Scores { get; set; }
	public decimal SumOfScores { get; set; }
	public decimal AverageScores { get; set; }
	public bool IsSuspended { get; set; }
}
```

#### ReadOnly model mapping configuration generation(for EF Core):
When we have readonly model, we might need to separate their DbContexts too. For example we might need to map models to main DbContext and map readonly models to readonly DbContext. 
In order to generate the readonly mapping configuration, consider we have the following mapping configuration:

```csharp
namespace MappingConfigurations;
[GenerateReadModelMappingConfiguration]
public class StudentMappingConfiguration : IEntityTypeConfiguration<StudentModel>
{
    public void Configure(EntityTypeBuilder<StudentModel> builder)
    {
        builder.ToTable("tblStudent", "demo");

        builder
            .HasMany(x => x.Scores)
            .WithOne(x => x.Student)
            .HasForeignKey(x => x.StudentId);

        //ReadModelMapping:IgnoreThisLine
        builder.Property(x => x.Name)
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        builder
            .HasMany(x => x.Courses)
            .WithMany(x => x.Students)
            .UsingEntity("tblStudentCourses",
            l => l.HasOne(typeof(StudentModel)).WithMany().HasForeignKey("StudentId"),
            r => r.HasOne(typeof(CourseModel)).WithMany().HasForeignKey("CourseId"))
            .ToTable("tblStudentCourses", "demo");
    }
}
```

`GenerateReadModelMappingConfiguration`: This is the main attribute should be added to class, which cause the readonly model mapping configuration generation.

`//ReadModelMapping:IgnoreThisLine`: Any statement which has this comment above it, will be ignored in transformation.

Finally above mentioned code will produced the following readonly model mapping configuration:

```csharp
using Jinget.Core.Contracts;
using Jinget.Core.Utilities.Expressions;
using Jinget.SourceGenerator.Common.Attributes;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MappingConfigurations;
public class ReadOnlyStudentMappingConfiguration: IEntityTypeConfiguration<Models.ReadOnlyStudentModel>
{
    public override void Configure(EntityTypeBuilder<Models.ReadOnlyStudentModel> builder)
    {
        builder.ToTable("tblStudent", "demo");
        builder.HasMany(x => x.Scores).WithOne(x => x.Student).HasForeignKey(x => x.StudentId);
        builder.HasMany(x => x.Courses).WithMany(x => x.Students).UsingEntity("tblStudentCourses", l => l.HasOne(typeof(Models.ReadOnlyStudentModel)).WithMany().HasForeignKey("StudentId"), r => r.HasOne(typeof(Models.ReadOnlyCourseModel)).WithMany().HasForeignKey("CourseId")).ToTable("tblStudentCourses", "demo");
    }
}
```

#### Web API generator:
Using this capability you can generate common web apis based the given models and readonly models. web apis generated by this generator is highly coupled to CQRS concepts.
In order to generate the web apis, consider we have the following mapping configuration:

```csharp
[GenerateWebAPI]
[GenerateReadModel(PreserveBaseTypes = true)]
[AppendPropertyToReadModel("bool", "IsSuspended")]
public class StudentModel : BaseEntity<int>, IAggregateRoot
{
    [PreserveOriginalGetterSetter]
    public string Name { get; private set; }

    [IgnoreReadModelConversion]
    public DateTime EnrollDate { get; set; }

    [PreserveOriginalType]
    public Address HomeAddress { get; set; }

    [Count("CoursesCount")]
    public ICollection<CourseModel> Courses { get; set; }

    [Sum(generatedPropertyName: "SumOfScores")]
    [Average(generatedPropertyName: "AverageScores")]
    public ICollection<StudentScoreModel> Scores { get; set; }
}
```

`GenerateWebAPI`: This is the main attribute should be added to class, which cause the web apis generation. In the above sample we have also included the readonly model generation too. 
This attribute has different arguments such as:

`HasClaim`: Indicates that the web apis has authorization claim.

`ClaimPrefix`: The prefix to use for the claim.

`ClaimTitle`: Instead of setting claim dynamically, this value will be used. final result will be `ClaimPrefix-ClaimTitle-create/modify etc`.

`ClaimType`: The claim type to use. You can introduce your own claim attribute(which inherits from Authohrize attribute)

`TenantAware`: Indicates that the model should be tenant aware, so that all queries should have tenant filter.

`CommandHandler`: The command handler interface to use. if interface is generic then write it like this: InterfaceName`NumberOfGenericArguments. for example for ITest[T1,T2] write ITest`2. Default value is IBaseCommandHandler`2

`QueryHandler`: The query handler interface to use. if interface is generic then write it like this: InterfaceName`NumberOfGenericArguments for example for ITest[T1,T2] write ITest`2. Default value is IBaseReadOnlyQueryHandler`2.

`TraceDataType`: The type to use for the trace data. if interface is generic then write it like this: InterfaceName`NumberOfGenericArguments for example for MyTraceData[T1,T2] write MyTraceData`2

`UserContextType`: The type to use for the user context. if interface is generic then write it like this: InterfaceName`NumberOfGenericArguments for example for MyTraceData[T1,T2] write MyTraceData`2

`Model`: The type to use for the model. if not specified, then model will be constructed using model in handlers.

`ReadOnlyModel`: The type to use for the readonly model. if not specified, then readonly model will be constructed using model in handlers.

Finally above mentioned code will produced the following readonly model:

```csharp
using Microsoft.AspNetCore.Routing;
using Mapster;
using System.Linq;
using System.Threading;
using Jinget.Core.Types;
using Jinget.Core.Utilities.Expressions;
using Jinget.Core.Types;
namespace WebApis;

public static class StudentApiExtensions 
{
    public static RouteGroupBuilder MapStudentApis(this RouteGroupBuilder group)
    {
        group.MapPost("/",  async (Handlers.IBaseCommandHandler<Models.StudentModel, int> command, Models.ReadOnlyStudentModel vm, CancellationToken cancellationToken) 
            => await command.SaveAsync(vm.Adapt<Models.StudentModel>(), cancellationToken: cancellationToken));

        group.MapPut("/",  async (Handlers.IBaseCommandHandler<Models.StudentModel, int> command, Models.ReadOnlyStudentModel vm, CancellationToken cancellationToken) 
            => await command.SaveAsync(vm.Adapt<Models.StudentModel>(), cancellationToken: cancellationToken));

        group.MapDelete("/{id}",  async (Handlers.IBaseCommandHandler<Models.StudentModel, int> command, int id, CancellationToken cancellationToken) 
            => await command.RemoveAsync(id, cancellationToken: cancellationToken));

        group.MapGet("/{id}",  async (Handlers.IBaseReadOnlyQueryHandler<Models.ReadOnlyStudentModel, int> query, int id, CancellationToken cancellationToken) 
            => await query.FetchAsync<Models.ReadOnlyStudentModel>(id, cancellationToken: cancellationToken));

        group.MapGet("/",  async (Handlers.IBaseReadOnlyQueryHandler<Models.ReadOnlyStudentModel, int> query, CancellationToken cancellationToken, int page=1, int pageSize=10, string sortColumn="id", string sortDirection="asc", string searchCriteria="") 
            => await query.FetchAllAsync<Models.ReadOnlyStudentModel>(
                new QueryOption<Models.ReadOnlyStudentModel>
                {
                    Filter = ExpressionUtility.CreateSearchAllColumnsExpression<Models.ReadOnlyStudentModel>(searchCriteria),
                    PaginationOption = new QueryOption<Models.ReadOnlyStudentModel>.PaginationOptionModel
                    {
                        PageIndex = page,
                        PageSize = pageSize
                    },
                    SortOption = new QueryOption<Models.ReadOnlyStudentModel>.SortOptionModel
                    {
                        SortColumn = sortColumn,
                        SortDirection = sortDirection
                    }
                }, cancellationToken: cancellationToken));

        group.MapPost("/delete/list",  async (Handlers.IBaseCommandHandler<Models.StudentModel, int> command, List<int> ids, CancellationToken cancellationToken) 
            => new ResponseResult<List<int>>(ids, (await command.RemoveRangeImmediateAsync(x => ids.Contains(x.Id), cancellationToken: cancellationToken)).EffectedRowsCount));

        return group;
    }
}
```

------------
# How to install
In order to install Jinget please refer to [nuget.org](https://www.nuget.org/packages/Jinget.SourceGenerator "nuget.org")

# Further Information
Sample codes are available via Unit Test projects which are provided beside the main source codes.

# Contact Me
👨‍💻 Twitter: https://twitter.com/_jinget

📧 Email: farahmandian2011@gmail.com

📣 Instagram: https://www.instagram.com/vahidfarahmandian