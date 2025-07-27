using Jinget.SourceGenerator.Common.Attributes;
using Jinget.SourceGenerator.Tests._BaseTypes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jinget.SourceGenerator.Tests._BaseTypes.MappingConfigurations;

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
