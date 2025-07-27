using Jinget.Core.Attributes.AggregationAttributes;
using Jinget.Core.Contracts;
using Jinget.SourceGenerator.Common.Attributes;
using System.Text.Json.Serialization;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

[GenerateWebAPI]
[GenerateReadModel]
public class CourseModel : IAggregateRoot
{
    public string Title { get; set; }

    [Count("StudentsCount")]
    [AppendAttributeToReadModel(typeof(JsonIgnoreAttribute))]
    public ICollection<StudentModel> Students { get; set; }
}