using Jinget.Core.Attributes.AggregationAttributes;
using Jinget.Core.Contracts;
using Jinget.SourceGenerator.Common.Attributes;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

[GenerateWebAPI]
[GenerateReadModel(PreserveBaseTypes = false, PreserveBaseInterfaces = true)]
[AppendPropertyToReadModel("bool", "IsSuspended", true)]
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

    [Count("CoursesCount", ignoreMapping: true)]
    [Count("CoursesCount2", ignoreMapping: false)]
    public ICollection<CourseModel> Courses { get; set; }

    [Sum(generatedPropertyName: "SumOfScores")]
    [Average(generatedPropertyName: "AverageScores")]
    [IgnoreMapping]
    [Min("MinScores")]
    [Max("MaxScores")]
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
public class ReadOnlyStudentModel : Jinget.SourceGenerator.Tests._BaseTypes.Models.ReadOnlyTraceBaseEntity<Jinget.SourceGenerator.Tests._BaseTypes.Models.Trace, int>, Jinget.Core.Contracts.IAggregateRoot, Jinget.Core.Contracts.ITenantAware, Jinget.Core.Contracts.IEntity
{
    public string Name { get; private set; }
    public Jinget.SourceGenerator.Tests._BaseTypes.Models.Gender Gender { get; set; }
    public Guid UniqueId { get; set; }
    public Jinget.SourceGenerator.Tests._BaseTypes.Models.Address HomeAddress { get; set; }
    public ICollection<Jinget.SourceGenerator.Tests._BaseTypes.Models.ReadOnlyCourseModel> Courses { get; set; }
    [Jinget.SourceGenerator.Common.Attributes.IgnoreMapping]
    public int CoursesCount { get; set; }
    public ICollection<Jinget.SourceGenerator.Tests._BaseTypes.Models.ReadOnlyStudentScoreModel> Scores { get; set; }
    public ICollection<string> OtherAttibutes { get; set; }
    [Jinget.SourceGenerator.Common.Attributes.IgnoreMapping]
    public decimal SumOfScores { get; set; }
    [Jinget.SourceGenerator.Common.Attributes.IgnoreMapping]
    public decimal AverageScores { get; set; }
    [Jinget.SourceGenerator.Common.Attributes.IgnoreMapping]
    public decimal Min_Scores_Scores { get; set; }
    [Jinget.SourceGenerator.Common.Attributes.IgnoreMapping]
    public decimal Max_Scores_Scores { get; set; }
    public string TenantId { get; set; }
    [Jinget.SourceGenerator.Common.Attributes.IgnoreMapping]
    public bool IsSuspended { get; set; }
}