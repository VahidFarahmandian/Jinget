namespace Jinget.Core.Attributes.AggregationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class MaxAttribute(string aggregatePropertyName, string? generatedPropertyName = null) : Attribute
{
    public string AggregatePropertyName { get; } = aggregatePropertyName;
    public string? GeneratedPropertyName { get; } = generatedPropertyName;
}