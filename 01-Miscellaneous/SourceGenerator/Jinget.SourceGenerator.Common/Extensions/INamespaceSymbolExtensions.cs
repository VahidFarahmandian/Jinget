using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class INamespaceSymbolExtensions
{
    internal static IEnumerable<string> GetNamespacesInAssembly(this INamespaceSymbol namespaceSymbol)
    {
        var namespaces = new HashSet<string>
        {
            namespaceSymbol.ToString()
        };

        foreach (var nestedNamespace in namespaceSymbol.GetMembers().OfType<INamespaceSymbol>())
        {
            namespaces.UnionWith(GetNamespacesInAssembly(nestedNamespace));
        }

        return namespaces;
    }
}
