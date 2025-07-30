using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Specifies properties that should be appended to the generated read model property
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class AppendPropertyToReadModelAttribute : Attribute
{
    public string TypeString { get; }
    public string PropertyName { get; }
    public bool IgnoreMapping { get; }

    public AppendPropertyToReadModelAttribute(string typeString, string propertyName, bool ignoreMapping = true)
    {
        if (string.IsNullOrWhiteSpace(typeString))
        {
            throw new ArgumentException($"'{nameof(typeString)}' cannot be null or empty.", nameof(typeString));
        }

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or empty.", nameof(propertyName));
        }

        TypeString = typeString;
        PropertyName = propertyName;
        IgnoreMapping = ignoreMapping;
    }
}