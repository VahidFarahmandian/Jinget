using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class AccessibilityExtensions
{
    internal static string StringfyAccessibility(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Friend or Accessibility.Internal => "internal",
            Accessibility.ProtectedAndFriend or
            Accessibility.ProtectedAndInternal or
            Accessibility.ProtectedOrFriend or
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => ""
        };
    }
}
