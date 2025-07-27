using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Attribute to mark a class to generate ReadOnlyModel mapping configuration for EF Core
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateReadModelMappingConfigurationAttribute : Attribute
{
    /// <summary>
    /// The type to use for the model.
    /// if not specified, then model will be constructed using model in handlers
    /// </summary>
    public string Model { get; set; }
}
