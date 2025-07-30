﻿namespace Jinget.Core.Attributes.AggregationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class AverageAttribute(string generatedPropertyName, bool ignoreMapping = true) :
    BaseAggregationAttribute(generatedPropertyName, ignoreMapping)
{
}