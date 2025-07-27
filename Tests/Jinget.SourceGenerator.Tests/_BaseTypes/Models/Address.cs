using Jinget.Core.Types.ValueObject;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

public class Address : JingetValueObject
{
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
}