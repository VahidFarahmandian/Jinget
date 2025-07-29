using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Indicates that the class should be used to generate a read model.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateReadModelAttribute : Attribute
{
    public string BaseType { get; set; } = "Object";
    public bool PreserveBaseTypes { get; set; } = false;
    public bool PreserveBaseInterfaces { get; set; } = false;
}
