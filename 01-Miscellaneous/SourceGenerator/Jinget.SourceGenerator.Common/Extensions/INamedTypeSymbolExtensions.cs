/// <summary>
/// Provides extension methods for <see cref="INamedTypeSymbol"/>.
/// </summary>
#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

/// <summary>
/// Provides extension methods for <see cref="INamedTypeSymbol"/>.
/// </summary>
internal static class INamedTypeSymbolExtensions
{
    public static bool IsSimpleType(this INamedTypeSymbol? type)
    {
        while (true)
        {
            if (IsPrimitive(type))
            {
                return true;
            }
            if (type != null && type.SpecialType == SpecialType.System_Decimal)
            {
                return true;
            }
            if (type != null && type.SpecialType == SpecialType.System_String)
            {
                return true;
            }
            if (type != null && type.ToString() == "System.Guid")
            {
                return true;
            }
            if (type != null && type.SpecialType == SpecialType.System_DateTime)
            {
                return true;
            }
            if (type != null && type.BaseType != null && type.BaseType.SpecialType == SpecialType.System_Enum)
            {
                return true;
            }
            if (type != null && type.TypeKind == TypeKind.Array && type.ToString() == "System.Byte[]")
            {
                return true;
            }
            if (type != null && type.ConstructedFrom.IsGenericType && type.ConstructedFrom.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                type = type.TypeArguments[0] as INamedTypeSymbol;
            }
            else
            {
                type = null;
            }
            if (type is null)
            {
                break;
            }
        }
        return false;
    }

    private static bool IsPrimitive(this INamedTypeSymbol? t)
    {
        if (t == null) return false;
        return t.SpecialType >= SpecialType.System_Boolean && t.SpecialType <= SpecialType.System_Double;
    }

    /// <summary>
    /// Retrieves the property marked with the "Key" attribute from the given type or its base types.
    /// </summary>
    /// <entity name="type">The <see cref="INamedTypeSymbol"/> to examine.</entity>
    /// <returns>The <see cref="IPropertySymbol"/> marked with the "Key" attribute, or <c>null</c> if not found.</returns>
    internal static IPropertySymbol? GetKeyProperty(this INamedTypeSymbol type)
    {
        while (type != null)
        {
            var keyProperty = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.HasAttribute("Key"));

            if (keyProperty != null)
            {
                return keyProperty;
            }

            if (type.BaseType == null)
                break;
            type = type.BaseType;
        }

        return null;
    }

    /// <summary>
    /// Gets the very base type (topmost parent) in the inheritance hierarchy.
    /// Returns null if the input is null or if the base type cannot be resolved.
    /// </summary>
    /// <entity name="type">The type to traverse upwards from.</entity>
    /// <returns>The innermost base type, or null if unreachable.</returns>
    internal static INamedTypeSymbol? GetRootBaseType(this INamedTypeSymbol? type)
    {
        if (type == null)
            return null;

        INamedTypeSymbol? current = type;
        while (current.BaseType != null)
        {
            // Stop if we've reached System.Object (or an unresolved base type)
            if (current.BaseType.SpecialType == SpecialType.System_Object ||
                current.BaseType.Name == "Object")
                break;

            current = current.BaseType;
        }

        return current;
    }

    internal static List<string> GetUsings(this INamedTypeSymbol? type)
    {
        List<string> usings = [];
        if (type == null)
            return usings;

        // 1. Get the SyntaxReference for the class declaration
        var classDeclaration = type.DeclaringSyntaxReferences.FirstOrDefault();
        if (classDeclaration == null)
            return []; // No syntax reference found

        // 2. Get the SyntaxTree
        var syntaxTree = classDeclaration.SyntaxTree;

        // 3. Get the root (CompilationUnitSyntax)
        var root = syntaxTree.GetRoot();
        var compilationUnit = root as CompilationUnitSyntax;

        // 4. Extract the using directives
        usings = compilationUnit?.Usings
            .Where(u => !u.HasIgnoreThisLineComment())
            .Select(u => u.Name == null ? "" : u.Name.ToString()).ToList() ?? [];

        // usings now contains all the imported namespaces
        return usings;
    }
}