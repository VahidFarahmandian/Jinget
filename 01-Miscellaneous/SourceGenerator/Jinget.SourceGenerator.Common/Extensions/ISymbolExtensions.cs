using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class ISymbolExtensions
{
    internal static bool IsIgnored(this ISymbol symbol)
    {
        return symbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.Name == "IgnoreReadModelConversion" ||
            attr.AttributeClass?.Name == "IgnoreReadModelConversionAttribute");
    }

    internal static bool PreserveOriginalType(this ISymbol symbol)
    {
        return symbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.Name == "PreserveOriginalType" ||
            attr.AttributeClass?.Name == "PreserveOriginalTypeAttribute");
    }

    internal static bool PreserveOriginalGetterSetter(this ISymbol symbol)
    {
        return symbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.Name == "PreserveOriginalGetterSetter" ||
            attr.AttributeClass?.Name == "PreserveOriginalGetterSetterAttribute");
    }

    internal static bool HasAttribute(this ISymbol type, string attribute)
    {
        attribute = attribute.EndsWith("Attribute") ?
            attribute.Substring(0, attribute.Length - "Attribute".Length) :
            attribute;
        return type.GetAttributes().Any(a => a.AttributeClass.Name == attribute || a.AttributeClass.Name == $"{attribute}Attribute");
    }

    internal static T GetAttributeNamedArgument<T>(this ISymbol type, Compilation compilation, string attribute, string namedArgument, object defaultValue)
    {
        attribute = attribute.EndsWith("Attribute") ?
            attribute.Substring(0, attribute.Length - "Attribute".Length) :
            attribute;

        var attributeData = type.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == $"{attribute}Attribute");
        if (attributeData != null)//attribute exists
        {
            var value = attributeData.NamedArguments.FirstOrDefault(a => a.Key == namedArgument).Value.Value;
            if (value == null)//attribute exists but no named argument called namedArgument is not set directly
            {
                var attributeSymbol = compilation.FindTypeInReferencedAssemblies(attributeData.AttributeClass.Name);

                // Use reflection to get the default value from the property
                var attributeType = Type.GetType($"{attributeSymbol}");
                var propertyInfo = attributeType.GetProperty(namedArgument);
                if (propertyInfo != null)
                {
                    return (T)Convert.ChangeType(propertyInfo.GetValue((Attribute)Activator.CreateInstance(attributeType)), typeof(T));
                }
            }
            return (T)Convert.ChangeType(attributeData.NamedArguments.FirstOrDefault(a => a.Key == namedArgument).Value.Value, typeof(T));
        }
        return (T)defaultValue;
    }
}
