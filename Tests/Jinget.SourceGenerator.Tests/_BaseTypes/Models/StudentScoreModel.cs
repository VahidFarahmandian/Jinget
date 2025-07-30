using Jinget.SourceGenerator.Common.Attributes;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

[GenerateReadModel]
public class StudentScoreModel
{
    public int StudentId { get; set; }

    public StudentModel Student { get; set; }
}

public class ReadOnlyStudentScoreModel
{
    public int StudentId { get; set; }

    public ReadOnlyStudentModel Student { get; set; }
}