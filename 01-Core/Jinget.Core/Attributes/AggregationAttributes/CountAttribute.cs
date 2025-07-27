namespace Jinget.Core.Attributes.AggregationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class CountAttribute(string? generatedPropertyName = null) : Attribute
{
    public string? GeneratedPropertyName { get; } = generatedPropertyName;
}