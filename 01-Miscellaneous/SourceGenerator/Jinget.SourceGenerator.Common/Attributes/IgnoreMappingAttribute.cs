using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Indicates that the property should be ignored when mapping in efcore mapping configuration
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class IgnoreMappingAttribute : Attribute
{
}
