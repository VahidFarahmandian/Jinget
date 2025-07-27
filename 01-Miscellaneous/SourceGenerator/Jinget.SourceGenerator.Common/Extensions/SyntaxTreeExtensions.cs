using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class SyntaxTreeExtensions
{
    internal static IEnumerable<ClassDeclarationSyntax> GetClasses(this SyntaxTree tree)
    {
        var root = tree.GetRoot() as CompilationUnitSyntax;
        List<ClassDeclarationSyntax> classes = [];
        var declarations = root?.DescendantNodes().OfType<ClassDeclarationSyntax>();
        if (declarations != null)
            classes.AddRange(declarations);
        return classes;
    }
    internal static IEnumerable<string> GetClassNames(this SyntaxTree tree)
    {
        return tree.GetClasses().Select(x => x.Identifier.Text);
    }

    internal static IEnumerable<MethodDeclarationSyntax> GetMethods(this SyntaxTree tree)
    {
        var root = tree.GetRoot() as CompilationUnitSyntax;
        List<MethodDeclarationSyntax> methods = [];
        var declarations = root?.DescendantNodes().OfType<MethodDeclarationSyntax>();
        if (declarations != null)
            methods.AddRange(declarations);
        return methods;
    }
    internal static IEnumerable<string> GetMethodNames(this SyntaxTree tree)
    {
        return tree.GetMethods().Select(x => x.Identifier.Text);
    }

    internal static IEnumerable<PropertyDeclarationSyntax> GetProperties(this SyntaxTree tree)
    {
        var root = tree.GetRoot() as CompilationUnitSyntax;
        List<PropertyDeclarationSyntax> properties = [];
        var declarations = root?.DescendantNodes().OfType<PropertyDeclarationSyntax>();
        if (declarations != null)
            properties.AddRange(declarations);
        return properties;
    }
    internal static IEnumerable<string> GetPropertyNames(this SyntaxTree tree)
    {
        return tree.GetProperties().Select(x => x.Identifier.Text);
    }

}
