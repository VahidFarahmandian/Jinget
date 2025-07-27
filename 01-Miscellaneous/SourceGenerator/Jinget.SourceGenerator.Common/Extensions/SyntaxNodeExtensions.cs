using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class SyntaxNodeExtensions
{
    //builder.MapColumnsByName<SampleModel, Guid>();
    internal static SyntaxNode ReplaceGenericArgumentsInMethodBody(this SyntaxNode node, Compilation compilation) => node.ReplaceNodes(
            nodes: node.DescendantNodes().OfType<GenericNameSyntax>(),
            computeReplacementNode: (originalNode, _) =>
            {
                var genericName = originalNode;
                if (genericName == null) return originalNode;

                var typeArguments = genericName.TypeArgumentList.Arguments;

                var newTypeArguments = typeArguments.Select(t =>
                {
                    string text = t.GetText().ToString().ToLower() == "string" ? $"System.String" : ((IdentifierNameSyntax)t).Identifier.Text;
                    var type = compilation.FindTypeInReferencedAssemblies(text);
                    if (type.IsSimpleType())
                    {
                        return SyntaxFactory.ParseTypeName($"{type.ContainingNamespace}.{type.Name}");
                    }
                    else
                    {
                        var rootBaseType = type.GetRootBaseType();

                        //do not change the type of trace-base entities
                        if (rootBaseType.ContainingNamespace.Name.Contains(".Jinget.") && rootBaseType.Name == "BaseTraceData")
                            return SyntaxFactory.ParseTypeName($"{type.ContainingNamespace}.{type.Name}");

                        return SyntaxFactory.ParseTypeName($"{type.ContainingNamespace}.ReadOnly{type.Name}");
                    }
                }).ToList();

                return genericName.WithTypeArgumentList(
                    SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(newTypeArguments)));
            });

    internal static SyntaxNode ReplaceModelNamesInMethodBody(this SyntaxNode node, Compilation compilation)
    {
        return node.ReplaceNodes(
               nodes: node.DescendantNodes().OfType<TypeOfExpressionSyntax>(),
               computeReplacementNode: (originalNode, _) =>
               {
                   var typeName = originalNode.Type.ToString();
                   var types = compilation.FindTypesInReferencedAssemblies(typeName);
                   string readOnlyTypeName = "";
                   foreach (var type in types)
                   {
                       readOnlyTypeName = $"{type.ContainingNamespace}.ReadOnly{typeName}";
                       try
                       {
                           compilation.FindTypeInReferencedAssemblies(readOnlyTypeName);
                           break;
                       }
                       catch (InvalidOperationException)//readOnlyTypeName is not found in the referenced assemblies
                       {
                           continue;
                       }
                   }

                   return SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseName(readOnlyTypeName));
               });
    }

    /// <summary>
    /// transform builder...HasForeignKey((MyModel x) => x.Id) to builder...HasForeignKey((ReadOnlyMyModel x) => x.Id)
    /// </summary>
    internal static SyntaxNode ReplaceModelNamesInLambda(this SyntaxNode node, Compilation compilation)
    {
        return node.ReplaceNodes(
              nodes: node.DescendantNodes().OfType<IdentifierNameSyntax>(),
              computeReplacementNode: (originalNode, _) =>
              {
                  if (originalNode.Identifier.Text.EndsWith("Model") && !originalNode.Identifier.Text.StartsWith("ReadOnly"))
                  {
                      INamedTypeSymbol symbol = null;
                      try
                      {
                          symbol = compilation.FindTypeInReferencedAssemblies(originalNode.Identifier.Text);
                          if (symbol != null && ImplementsIEntity(compilation, symbol) && !HasReadOnlyModelAttribute(compilation, symbol))
                          {
                              var readOnlyTypeName = $"{symbol.ContainingNamespace}.ReadOnly{symbol.Name}";
                              return SyntaxFactory.IdentifierName(readOnlyTypeName);
                          }
                      }
                      catch { }
                  }

                  return originalNode;
              });
    }
    private static bool ImplementsIEntity(Compilation compilation, INamedTypeSymbol type)
    {
        var iEntitySymbol = compilation.FindTypeInReferencedAssemblies("Jinget.Core.Contracts.IEntity");
        return type.AllInterfaces.Contains(iEntitySymbol);
    }
    private static bool HasReadOnlyModelAttribute(Compilation compilation, INamedTypeSymbol type)
    {
        var readOnlyModelAttributeSymbol = compilation.FindTypeInReferencedAssemblies("Jinget.Core.Attributes.ReadOnlyModelAttribute");

        // Check current type and all base types
        INamedTypeSymbol? currentType = type;
        while (currentType != null)
        {
            if (currentType.GetAttributes()
                .Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, readOnlyModelAttributeSymbol)))
            {
                return true;
            }

            currentType = currentType.BaseType;
        }

        return false;
    }

    internal static SyntaxNode ReplaceIgnoredStatements(this SyntaxNode node)
    {
        var statementsToRemove = node.DescendantNodes()
            .OfType<StatementSyntax>()
            .Where(x => x.HasIgnoreThisLineComment())
            .ToList();

        return node.RemoveNodes(statementsToRemove, SyntaxRemoveOptions.KeepDirectives)!;
    }

    internal static SyntaxNode RemoveAllComments(this SyntaxNode node)
    {
        return node.ReplaceTrivia(
             node.DescendantTrivia().Where(t =>
                t.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                t.IsKind(SyntaxKind.MultiLineCommentTrivia)),
            (originalTrivia, _) => default);
    }

    internal static SyntaxNode RemoveExtraWhitespace(this SyntaxNode node) => node.NormalizeWhitespace();

    internal static SyntaxNode RemoveEmptyLines(this SyntaxNode node)
    {
        return node.ReplaceTrivia(
            node.DescendantTrivia().Where(t => t.IsKind(SyntaxKind.EndOfLineTrivia)),
            (originalTrivia, _) => SyntaxFactory.Space);
    }
}
