namespace Jinget.Core.Attributes.AggregationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class MaxAttribute(string aggregatePropertyName, string? generatedPropertyName = null, bool ignoreMapping = true) :
    BaseAggregationAttribute(generatedPropertyName, ignoreMapping)
{
    public string AggregatePropertyName { get; } = aggregatePropertyName;
}