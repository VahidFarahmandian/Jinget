using Microsoft.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class ITypeSymbolExtensions
{
    internal static bool IsArrayType(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind == TypeKind.Array;
    }

    internal static bool IsPrimitiveOrSpecialType(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsValueType ||
               typeSymbol.SpecialType == SpecialType.System_String ||
               typeSymbol.Name == "Guid" ||
               typeSymbol.Name == "HierarchyId";
    }

    internal static bool IsComplexType(this ITypeSymbol typeSymbol)
    {
        return !typeSymbol.IsPrimitiveOrSpecialType();
    }

    internal static bool IsCollectionType(this ITypeSymbol typeSymbol)
    {
        // Handle array types separately
        if (typeSymbol.TypeKind == TypeKind.Array)
            return false; // Arrays are handled by IsArrayType()

        // Check for generic collection interfaces
        if (typeSymbol is INamedTypeSymbol namedType)
        {
            // Case 1: Directly implements ICollection<T>/IList<T>/IEnumerable<T>
            var interfaces = namedType.AllInterfaces;
            if (interfaces.Any(i =>
                i.MetadataName == "ICollection`1" ||
                i.MetadataName == "IList`1" ||
                i.MetadataName == "IEnumerable`1"))
            {
                return true;
            }

            // Case 2: Is itself a generic collection interface
            var metadataName = namedType.MetadataName;
            if (metadataName == "ICollection`1" ||
                metadataName == "IList`1" ||
                metadataName == "IEnumerable`1")
            {
                return true;
            }

            // Case 3: Concrete collection types (List<T>, HashSet<T>, etc.)
            var originalDefinition = namedType.OriginalDefinition?.ToString();
            if (originalDefinition is not null && (
                originalDefinition.StartsWith("List<") ||
                originalDefinition.StartsWith("HashSet<") ||
                originalDefinition.StartsWith("ICollection<") ||
                originalDefinition.StartsWith("IEnumerable<")))
            {
                return true;
            }
        }

        return false;

    }
}
