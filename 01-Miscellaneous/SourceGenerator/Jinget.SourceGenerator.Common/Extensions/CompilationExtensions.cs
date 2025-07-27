#nullable enable
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class CompilationExtensions
{
    internal static INamedTypeSymbol? FindTypeInReferencedAssemblies(this Compilation compilation, string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        // Create a list of all assemblies to search (including the calling assembly)
        var assembliesToSearch = new List<IAssemblySymbol> { compilation.Assembly };
        var referencedAssemblies = compilation.References.Select(r => compilation.GetAssemblyOrModuleSymbol(r) as IAssemblySymbol).Where(a => a != null);
        if (referencedAssemblies != null)
            assembliesToSearch.AddRange(referencedAssemblies);

        // If not found in calling assembly, proceed with searching referenced assemblies
        foreach (var assembly in assembliesToSearch)
        {
            // find type in root of assembly(global namespace)
            var typeWithoutNamespaceSymbol = assembly.GetTypeByMetadataName($"{typeName}");
            if (typeWithoutNamespaceSymbol != null)
            {
                return typeWithoutNamespaceSymbol;
            }

            // find type in default namespace(namespace equal to assembly name)
            var typeSymbol = assembly.GetTypeByMetadataName($"{assembly.Name}.{typeName}");
            if (typeSymbol != null)
            {
                return typeSymbol;
            }

            // find type in nested namespaces inside assembly
            foreach (var nestedNamespace in assembly.GlobalNamespace.GetNamespacesInAssembly())
            {
                var fullTypeName = $"{nestedNamespace}.{typeName}";
                typeSymbol = compilation.GetTypeByMetadataName(fullTypeName);
                if (typeSymbol != null)
                {
                    return typeSymbol;
                }
            }
        }
        throw new InvalidOperationException($"{typeName} not found in the current and referenced assemblies." +
            $" Referenced assemblies: \n" +
            $"{string.Join("\n", assembliesToSearch.Select(x => x.Name))}");
    }

    internal static List<INamedTypeSymbol>? FindTypesInReferencedAssemblies(this Compilation compilation, string typeName)
    {
        List<INamedTypeSymbol>? types = null;

        if (string.IsNullOrWhiteSpace(typeName))
            return types;
        types = [];
        // Create a list of all assemblies to search (including the calling assembly)
        var assembliesToSearch = new List<IAssemblySymbol> { compilation.Assembly };
        assembliesToSearch.AddRange(compilation.References.Select(r => compilation.GetAssemblyOrModuleSymbol(r) as IAssemblySymbol).Where(a => a != null));

        // If not found in calling assembly, proceed with searching referenced assemblies
        foreach (var assembly in assembliesToSearch)
        {
            // find type in root of assembly(global namespace)
            var typeWithoutNamespaceSymbol = assembly.GetTypeByMetadataName($"{typeName}");
            if (typeWithoutNamespaceSymbol != null)
            {
                types.Add(typeWithoutNamespaceSymbol);
            }

            // find type in default namespace(namespace equal to assembly name)
            var typeSymbol = assembly.GetTypeByMetadataName($"{assembly.Name}.{typeName}");
            if (typeSymbol != null)
            {
                types.Add(typeSymbol);
            }

            // find type in nested namespaces inside assembly
            foreach (var nestedNamespace in assembly.GlobalNamespace.GetNamespacesInAssembly())
            {
                var fullTypeName = $"{nestedNamespace}.{typeName}";
                typeSymbol = compilation.GetTypeByMetadataName(fullTypeName);
                if (typeSymbol != null)
                {
                    types.Add(typeSymbol);
                }
            }
        }
        if (types.Any())
            return types;
        throw new InvalidOperationException($"{typeName} not found in the current and referenced assemblies." +
            $" Referenced assemblies: \n" +
            $"{string.Join("\n", assembliesToSearch.Select(x => x.Name))}");
    }

}
