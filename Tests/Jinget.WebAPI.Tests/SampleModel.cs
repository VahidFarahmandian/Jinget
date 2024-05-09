using Jinget.Core.Attributes;

namespace Jinget.WebAPI.Tests;

public class SampleModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }

    [SwaggerExclude]
    public string? FullName { get; set; }
}
