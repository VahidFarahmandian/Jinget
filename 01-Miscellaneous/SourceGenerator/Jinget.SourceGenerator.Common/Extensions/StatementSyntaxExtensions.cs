using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class StatementSyntaxExtensions
{
    internal static bool HasIgnoreThisLineComment(this CSharpSyntaxNode statement)
    {
        return statement
            .GetLeadingTrivia()
            .Any(t =>
            t.IsKind(SyntaxKind.SingleLineCommentTrivia) &&
            (t.ToString().TrimStart('/').TrimStart().StartsWith("ReadModelMapping:IgnoreThisLine", StringComparison.OrdinalIgnoreCase) ||
            t.ToString().TrimStart('/').TrimStart().StartsWith("ReadModelMapping: IgnoreThisLine", StringComparison.OrdinalIgnoreCase)));
    }
}
