using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Indicates that the class/property should be ignored when generating a read model.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class IgnoreReadModelConversionAttribute : Attribute
{
}
