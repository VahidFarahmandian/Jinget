namespace Jinget.Core.Attributes.AggregationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class SumAttribute(string? aggregatePropertyName = null, string? generatedPropertyName = null, bool ignoreMapping = true) :
    BaseAggregationAttribute(generatedPropertyName, ignoreMapping)
{
    public string? AggregatePropertyName { get; } = aggregatePropertyName;
}