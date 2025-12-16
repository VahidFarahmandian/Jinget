using Jinget.Core.Contracts;
using Jinget.SourceGenerator.Common.Attributes;

using Microsoft.AspNetCore.Http;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

[GenerateWebAPI]
[GenerateReadModel(PreserveBaseTypes = false, PreserveBaseInterfaces = true)]
[AppendPropertyToReadModel("bool", "IsSuspended", true)]
[AppendPropertyToReadModel("IFormFile", "File", true, "private get;", "set;")]
[AppendPropertyToReadModel("IEntity", "Entity", true)]
[AppendAttributeToReadModel("CacheTypeIdentifier(\"sample\")")]
public class StudentModel : TraceBaseEntity<Trace, int>, IAggregateRoot, ITenantAware, IEntity
{
    [PreserveOriginalGetterSetter]
    public string Name { get; private set; }

    public Gender Gender { get; private set; }

    [IgnoreMapping]
    public Guid UniqueId { get; set; }

    [IgnoreReadModelConversion]
    public DateTime EnrollDate { get; set; }

    [PreserveOriginalType]
    public Address HomeAddress { get; set; }

    public ICollection<CourseModel> Courses { get; set; }

    [IgnoreMapping]
    public ICollection<StudentScoreModel> Scores { get; set; }

    public ICollection<string> OtherAttibutes { get; set; }

    public string TenantId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CacheTypeIdentifier : Attribute
{
    public string Identifier { get; set; }

    public CacheTypeIdentifier(string identifier)
    {
        Identifier = identifier;
    }
}
[Jinget.SourceGenerator.Tests._BaseTypes.Models.CacheTypeIdentifier("sample")]
public class ReadOnlyStudentModel : Jinget.SourceGenerator.Tests._BaseTypes.Models.ReadOnlyTraceBaseEntity<Jinget.SourceGenerator.Tests._BaseTypes.Models.Trace, int>, Jinget.Core.Contracts.IAggregateRoot, Jinget.Core.Contracts.ITenantAware, Jinget.Core.Contracts.IEntity
{
    public string Name { get; private set; }
    public Jinget.SourceGenerator.Tests._BaseTypes.Models.Gender Gender { get; set; }
    public Guid UniqueId { get; set; }
    public Jinget.SourceGenerator.Tests._BaseTypes.Models.Address HomeAddress { get; set; }
    public ICollection<Jinget.SourceGenerator.Tests._BaseTypes.Models.ReadOnlyCourseModel> Courses { get; set; }
    public ICollection<Jinget.SourceGenerator.Tests._BaseTypes.Models.ReadOnlyStudentScoreModel> Scores { get; set; }
    public ICollection<string> OtherAttibutes { get; set; }
    public string TenantId { get; set; }
    public bool IsSuspended { get; set; }
    public IFormFile File { private get; set; }
    public Jinget.Core.Contracts.IEntity Entity { get; set; }
}