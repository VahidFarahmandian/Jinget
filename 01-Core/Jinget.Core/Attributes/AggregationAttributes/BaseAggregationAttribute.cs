namespace Jinget.Core.Attributes.AggregationAttributes;

public abstract class BaseAggregationAttribute(string? generatedPropertyName = null, bool ignoreMapping = true) : Attribute
{
    public string? GeneratedPropertyName { get; } = generatedPropertyName;
    public bool ignoreMapping { get; } = ignoreMapping;
}