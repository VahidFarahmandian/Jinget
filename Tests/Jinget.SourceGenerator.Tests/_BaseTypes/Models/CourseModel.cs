using Jinget.Core.Contracts;
using Jinget.SourceGenerator.Common.Attributes;

using System.Text.Json.Serialization;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

[GenerateWebAPI]
[GenerateReadModel]
[AppendPropertyToReadModel("int", "StudentsCount")]
public class CourseModel : IAggregateRoot
{
    public string Title { get; set; }

    [AppendAttributeToProperty(typeof(JsonIgnoreAttribute))]
    public ICollection<StudentModel> Students { get; set; }
}

public class ReadOnlyCourseModel : IAggregateRoot
{
    public string Title { get; set; }

    [IgnoreMapping]
    public int StudentsCount { get; set; }
    [JsonIgnore]
    public ICollection<ReadOnlyStudentModel> Students { get; set; }
}