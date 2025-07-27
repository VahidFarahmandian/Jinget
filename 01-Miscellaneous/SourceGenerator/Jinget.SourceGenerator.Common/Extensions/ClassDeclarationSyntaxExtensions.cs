using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class ClassDeclarationSyntaxExtensions
{
    internal static bool HasAttribute(this ClassDeclarationSyntax classDeclaration, string attributeName)
    {
        attributeName = attributeName.EndsWith("Attribute")
                ? attributeName.Substring(0, attributeName.Length - "Attribute".Length)
                : attributeName;

        return classDeclaration.AttributeLists.Any(al => al.Attributes.Any(a =>
            a.Name.ToString() == attributeName ||
            a.Name.ToString() == $"{attributeName}Attribute"));
    }
}
