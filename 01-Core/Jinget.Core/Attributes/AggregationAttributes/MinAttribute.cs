namespace Jinget.Core.Attributes.AggregationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class MinAttribute(string aggregatePropertyName, string? generatedPropertyName = null) : Attribute
{
    public string AggregatePropertyName { get; } = aggregatePropertyName;
    public string? GeneratedPropertyName { get; } = generatedPropertyName;
}