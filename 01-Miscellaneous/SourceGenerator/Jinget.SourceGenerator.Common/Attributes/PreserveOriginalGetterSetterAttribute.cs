using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Indicates that the original getter/setter access modifiers should be preserved when generating a read model.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class PreserveOriginalGetterSetterAttribute : Attribute
{
}
