using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator")]
[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Common.Extensions;

internal static class SyntaxNodeExtensions
{
    private static string GetFullTypeNameFromKeyword(string keyword)
    {
        return keyword.ToLower() switch
        {
            "bool" => "System.Boolean",
            "byte" => "System.Byte",
            "guid" => "System.Guid",
            "sbyte" => "System.SByte",
            "char" => "System.Char",
            "decimal" => "System.Decimal",
            "double" => "System.Double",
            "float" => "System.Single",
            "int" => "System.Int32",
            "uint" => "System.UInt32",
            "long" => "System.Int64",
            "ulong" => "System.UInt64",
            "short" => "System.Int16",
            "ushort" => "System.UInt16",
            "object" => "System.Object",
            "string" => "System.String",
            "dynamic" => "System.Object",
            _ => keyword,// fallback
        };
    }

    //builder.MapColumnsByName<SampleModel, Guid>();
    internal static SyntaxNode ReplaceGenericArgumentsInMethodBody(this SyntaxNode node, Compilation compilation) => node.ReplaceNodes(
     nodes: node.DescendantNodes().OfType<GenericNameSyntax>(),
     computeReplacementNode: (originalNode, _) =>
     {
         var genericName = originalNode;
         if (genericName == null) return originalNode;

         var newTypeArguments = genericName.TypeArgumentList.Arguments.Select(t =>
         {
             return ProcessTypeSyntax(t, compilation);
         }).ToList();

         return genericName.WithTypeArgumentList(
             SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(newTypeArguments)));
     });

    private static TypeSyntax ProcessTypeSyntax(TypeSyntax typeSyntax, Compilation compilation)
    {
        switch (typeSyntax)
        {
            case GenericNameSyntax genericNameSyntax:
                return ProcessGenericNameSyntax(genericNameSyntax, compilation);

            case IdentifierNameSyntax identifierNameSyntax:
                return ProcessIdentifierNameSyntax(identifierNameSyntax, compilation);

            case PredefinedTypeSyntax predefinedTypeSyntax:
                return ProcessPredefinedTypeSyntax(predefinedTypeSyntax, compilation);

            case QualifiedNameSyntax qualifiedNameSyntax:
                return ProcessQualifiedNameSyntax(qualifiedNameSyntax, compilation);

            default:
                return typeSyntax;
        }
    }

    private static TypeSyntax ProcessGenericNameSyntax(GenericNameSyntax genericNameSyntax, Compilation compilation)
    {
        var identifier = genericNameSyntax.Identifier.Text;
        var type = compilation.FindTypeInReferencedAssemblies(identifier);

        // Handle System.Collections types - don't change the container type, but process its arguments
        if (type != null && type.ContainingNamespace.ToString().StartsWith("System.Collections"))
        {
            var newTypeArguments = genericNameSyntax.TypeArgumentList.Arguments.Select(arg =>
                ProcessTypeSyntax(arg, compilation)
            ).ToList();

            return SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(identifier),
                SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(newTypeArguments))
            );
        }
        else
        {
            throw new InvalidOperationException($"Jinget SG Says: Generic type {identifier} is not supported in generic type arguments.");
        }
    }

    private static TypeSyntax ProcessIdentifierNameSyntax(IdentifierNameSyntax identifierNameSyntax, Compilation compilation)
    {
        var text = identifierNameSyntax.Identifier.Text;
        var type = compilation.FindTypeInReferencedAssemblies(text);

        if (type == null)
            return identifierNameSyntax;

        return ShouldConvertToReadOnly(type)
            ? SyntaxFactory.IdentifierName($"{type.ContainingNamespace}.ReadOnly{type.Name}")
            : SyntaxFactory.IdentifierName($"{type.ContainingNamespace}.{type.Name}");
    }

    private static TypeSyntax ProcessPredefinedTypeSyntax(PredefinedTypeSyntax predefinedTypeSyntax, Compilation compilation)
    {
        var text = GetFullTypeNameFromKeyword(predefinedTypeSyntax.ToString());
        var type = compilation.FindTypeInReferencedAssemblies(text);

        if (type == null)
            return predefinedTypeSyntax;

        return SyntaxFactory.ParseTypeName($"{type.ContainingNamespace}.{type.Name}");
    }

    private static TypeSyntax ProcessQualifiedNameSyntax(QualifiedNameSyntax qualifiedNameSyntax, Compilation compilation)
    {
        // For qualified names like "System.Collections.Generic.ICollection<T>"
        // We need to handle both the left and right parts
        var left = ProcessTypeSyntax(qualifiedNameSyntax.Left, compilation);
        var right = ProcessTypeSyntax(qualifiedNameSyntax.Right, compilation);

        return SyntaxFactory.QualifiedName((NameSyntax)left, (SimpleNameSyntax)right);
    }

    private static bool ShouldConvertToReadOnly(INamedTypeSymbol type)
    {
        var rootBaseType = type.GetRootBaseType();

        // Don't convert simple types
        if (type.IsSimpleType())
            return false;

        // Don't convert trace-base entities
        if (rootBaseType.ContainingNamespace.Name.Contains(".Jinget.") && rootBaseType.Name == "BaseTraceData")
            return false;

        // Don't convert System.Collections types (they're containers, not the actual types we want to convert)
        if (type.ContainingNamespace.ToString().StartsWith("System.Collections"))
            return false;

        // Convert everything else to ReadOnly
        return true;
    }

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

                              string readOnlyTypeName = "";

                              // If the original type is already a ReadOnly type, don't prefix again
                              if (symbol.Name.StartsWith("ReadOnly"))
                                  readOnlyTypeName = $"{symbol.ContainingNamespace}.{symbol.Name}";
                              else
                                  readOnlyTypeName = $"{symbol.ContainingNamespace}.ReadOnly{symbol.Name}";
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

    internal static SyntaxNode AddIgnoreMappingStatements(this BlockSyntax node, List<string> ignoredProperties)
    {
        if (ignoredProperties == null || ignoredProperties.Any() == false)
            return node;

        var newBody = node.AddStatements(
            [.. ignoredProperties.Select(prop =>
                SyntaxFactory.ParseStatement($"\t\tbuilder.Ignore(\"{prop}\");\r\n")
            )]
        );
        return newBody;
    }
}
