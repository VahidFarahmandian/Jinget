namespace Jinget.Core.Attributes.AggregationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class CountAttribute(string? generatedPropertyName = null, bool ignoreMapping = true) : 
    BaseAggregationAttribute(generatedPropertyName, ignoreMapping)
{
}