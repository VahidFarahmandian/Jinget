using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Indicates custom getter/setter when generating a read model.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class CustomGetterSetterAttribute : Attribute
{
    public string? Getter { get; set; }
    public string? Setter { get; set; }
}
