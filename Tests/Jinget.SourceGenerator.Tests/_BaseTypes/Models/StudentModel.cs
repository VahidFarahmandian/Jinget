using Jinget.Core.Attributes.AggregationAttributes;
using Jinget.Core.Contracts;
using Jinget.SourceGenerator.Common.Attributes;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

[GenerateWebAPI]
[GenerateReadModel(PreserveBaseTypes = true, PreserveBaseInterfaces = true)]
[AppendPropertyToReadModel("bool", "IsSuspended")]
public class StudentModel : BaseEntity<int>, IAggregateRoot, ITenantAware
{
    [PreserveOriginalGetterSetter]
    public string Name { get; private set; }

    public Gender Gender { get; private set; }

    public Guid UniqueId { get; set; }

    [IgnoreReadModelConversion]
    public DateTime EnrollDate { get; set; }

    [PreserveOriginalType]
    public Address HomeAddress { get; set; }

    [Count("CoursesCount")]
    public ICollection<CourseModel> Courses { get; set; }

    [Sum(generatedPropertyName: "SumOfScores")]
    [Average(generatedPropertyName: "AverageScores")]
    public ICollection<StudentScoreModel> Scores { get; set; }
    public string TenantId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}