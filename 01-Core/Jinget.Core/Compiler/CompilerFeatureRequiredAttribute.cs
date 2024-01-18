namespace System.Runtime.CompilerServices;

/// <summary>
/// add support for 'required' keyword using latest lang version and .net standard 2.1
/// </summary>
#pragma warning disable CA1018 // Mark attributes with AttributeUsageAttribute
public class CompilerFeatureRequiredAttribute(string name) : Attribute
#pragma warning restore CA1018 // Mark attributes with AttributeUsageAttribute
{
}