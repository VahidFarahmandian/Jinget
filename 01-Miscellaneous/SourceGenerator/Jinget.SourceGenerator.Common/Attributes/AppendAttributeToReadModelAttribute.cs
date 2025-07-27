using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Specifies attributes that should be applied to the generated read model property
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public sealed class AppendAttributeToReadModelAttribute : Attribute
{
    /// <summary>
    /// The attribute to apply to the read model property
    /// Can be either:
    /// - Simple attribute name ("JsonIgnore")
    /// - Full attribute with parameters ("OtherAttribute(""someParam"")")
    /// </summary>
    public string AttributeText { get; }

    public AppendAttributeToReadModelAttribute(string attributeText)
    {
        if (string.IsNullOrWhiteSpace(attributeText))
        {
            throw new ArgumentNullException(nameof(attributeText));
        }
        AttributeText = attributeText;
    }

    // Alternative constructor that takes a Type
    public AppendAttributeToReadModelAttribute(Type attributeType)
    {
        if (attributeType == null)
            throw new ArgumentNullException(nameof(attributeType));

        if (!attributeType.IsSubclassOf(typeof(Attribute)))
            throw new ArgumentException("Type must be an Attribute", nameof(attributeType));

        AttributeText = attributeType.Name.EndsWith("Attribute")
            ? attributeType.Name.Substring(0, attributeType.Name.Length - 9)
            : attributeType.Name;
    }
}