namespace Jinget.Core.Attributes.AggregationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class SumAttribute(string? aggregatePropertyName = null, string? generatedPropertyName = null) : Attribute
{
    public string? AggregatePropertyName { get; } = aggregatePropertyName;
    public string? GeneratedPropertyName { get; } = generatedPropertyName;
}