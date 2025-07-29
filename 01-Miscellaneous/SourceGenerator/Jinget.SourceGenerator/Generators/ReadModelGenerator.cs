using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Generators;

[GeneratorOrder(Order = 1)]
public class ReadModelGenerator : IIncrementalGenerator
{
    private static readonly HashSet<string> ValidPrimitiveTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "bool", "byte", "sbyte", "char", "decimal", "double", "float",
        "int", "uint", "long", "ulong", "short", "ushort", "object",
        "string", "dynamic", "DateTime", "DateTimeOffset", "TimeSpan", "Guid"
    };

    private static readonly string[] AggregationAttributes =
        ["CountAttribute", "SumAttribute", "AverageAttribute", "MinAttribute", "MaxAttribute"];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var entityDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (c, _) => GetSemanticTargetForGeneration(c))
            .Where(static data => data != null)
            .Collect();

        var compilationWithEntities = context.CompilationProvider.Combine(entityDeclarations);

        context.RegisterSourceOutput(
            compilationWithEntities,
            static (context, data) => GenerateAndAddSources(context, data.Left, data.Right));
    }

    internal static ImmutableArray<(string HintName, string Code)> GenerateReadModelSources(Compilation compilation, ImmutableArray<INamedTypeSymbol> classes)
    {
        if (compilation is null) throw new ArgumentNullException(nameof(compilation));

        return [.. classes
            .Where(type => !type.IsIgnored())
            .Select(type => GenerateReadModelForType(compilation, type))];
    }

    private static void GenerateAndAddSources(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<INamedTypeSymbol> classes)
    {
        foreach (var (HintName, Code) in GenerateReadModelSources(compilation, classes))
        {
            context.AddSource($"{HintName}.g.cs",
                SourceText.From(Code, Encoding.UTF8));
        }
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is ClassDeclarationSyntax classDeclaration &&
        classDeclaration.HasAttribute("GenerateReadModel");

    private static INamedTypeSymbol GetSemanticTargetForGeneration(GeneratorSyntaxContext context) =>
        context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)context.Node)!;

    private static (string HintName, string Code) GenerateReadModelForType(
        Compilation compilation,
        INamedTypeSymbol type)
    {
        var className = $"ReadOnly{type.Name}";
        var inheritedTypes = GetInheritanceString(type, compilation);
        var properties = GeneratePropertiesForType(type);
        var newProperties = GetAppendedPropertiesFromAttributes(type);
        var usingNamespaces = GetBaseNamespaces(type, compilation);

        var classContent = $$"""
            {{usingNamespaces}}
            namespace {{type.ContainingNamespace}};
            public class {{className}} : {{inheritedTypes}}
            {
                {{string.Join("\r\n\t", properties.Concat(newProperties))}}
            }
            """;

        return (className, classContent);
    }
    private static string GetBaseNamespaces(INamedTypeSymbol type, Compilation compilation)
    {
        List<string> namespaces = [];
        if (type.BaseType != null && type.BaseType.ContainingNamespace.ToDisplayString() != type.ContainingNamespace.ToDisplayString())
            namespaces.Add($"using {type.BaseType.ContainingNamespace.ToDisplayString()};");

        var preserveBaseInterfaces = type.GetAttributeNamedArgument<bool>(
            compilation, "GenerateReadModel", "PreserveBaseInterfaces", false);
        if (preserveBaseInterfaces)
        {
            foreach (var i in type.Interfaces)
            {
                if (i.ContainingNamespace.ToDisplayString() != type.ContainingNamespace.ToDisplayString())
                    namespaces.Add($"using {i.ContainingNamespace.ToDisplayString()};");
            }
        }

        return string.Join("\r\n", namespaces.Distinct());
    }

    private static string GetInheritanceString(INamedTypeSymbol type, Compilation compilation)
    {
        var preserveBaseTypes = type.GetAttributeNamedArgument<bool>(
            compilation, "GenerateReadModel", "PreserveBaseTypes", false);

        string baseType = "";
        if (preserveBaseTypes)
        {
            baseType = type.BaseType == null ? "Object" : type.BaseType.ToDisplayString();
        }
        else
        {
            baseType = type.GetAttributeNamedArgument<string>(
                compilation, "GenerateReadModel", "BaseType", "Object");

            baseType = type.BaseType switch
            {
                { Name: "BaseEntity", TypeArguments.Length: 1 } =>
                    $"BaseReadOnlyEntity<{type.BaseType.TypeArguments[0].ToDisplayString()}>",
                { Name: "TraceBaseEntity", TypeArguments.Length: 3 } =>
                    $"TraceBaseReadOnlyEntity<{type.BaseType.TypeArguments[0].ToDisplayString()}," +
                    $"{type.BaseType.TypeArguments[1].ToDisplayString()}," +
                    $"{type.BaseType.TypeArguments[2].ToDisplayString()}>",
                _ => baseType
            };
        }

        IEnumerable<string> interfaces = [];
        var preserveBaseInterfaces = type.GetAttributeNamedArgument<bool>(
            compilation, "GenerateReadModel", "PreserveBaseInterfaces", false);
        if (preserveBaseInterfaces)
        {
            interfaces = type.AllInterfaces.Select(i => i.ToDisplayString());
        }
        else
        {
            interfaces = type.AllInterfaces
                .Where(x => x.Name == "ITenantAware")
                .Select(i => i.ToDisplayString());
        }

        return string.Join(", ", new[] { baseType }.Concat(interfaces).Where(s => !string.IsNullOrEmpty(s)).Distinct());
    }

    private static IEnumerable<string> GeneratePropertiesForType(INamedTypeSymbol type)
    {
        return type.GetMembers()
            .Where(m => m.Kind == SymbolKind.Property)
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsIgnored())
            .SelectMany(GeneratePropertyDefinitions);
    }

    private static IEnumerable<string> GeneratePropertyDefinitions(IPropertySymbol property)
    {
        var propertyDefinitions = new List<string>();

        // Add attributes
        var attributes = ProcessReadModelAttributes(property);
        if (!string.IsNullOrEmpty(attributes))
        {
            propertyDefinitions.Add(attributes.Trim());
        }

        // Generate property declaration
        propertyDefinitions.Add(GeneratePropertyDeclaration(property));

        // Add aggregation properties
        propertyDefinitions.AddRange(GenerateAggregationProperties(property));

        return propertyDefinitions;
    }

    private static string GeneratePropertyDeclaration(IPropertySymbol property)
    {
        var modifiers = GetPropertyModifiers(property);
        var type = GetPropertyTypeString(property);
        var accessors = GetPropertyAccessors(property);

        return $"{modifiers} {type} {property.Name} {{ {accessors} }}";
    }

    private static string GetPropertyModifiers(IPropertySymbol property)
    {
        var modifiers = property.DeclaredAccessibility.StringfyAccessibility();
        if (property.IsOverride) modifiers += " override";
        if (property.IsVirtual) modifiers += " virtual";
        return modifiers;
    }

    private static string GetPropertyTypeString(IPropertySymbol property)
    {
        if (property.Type.IsPrimitiveOrSpecialType() || property.PreserveOriginalType())
            return property.Type.ToDisplayString();

        if (property.Type.IsCollectionType())
            return GetCollectionTypeString(property);

        if (property.Type.IsArrayType())
            return GetArrayTypeString(property);

        return $"{property.Type.ContainingNamespace}.ReadOnly{property.Type.Name}" +
               (property.Type.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "");
    }

    private static string GetCollectionTypeString(IPropertySymbol property)
    {
        var namedType = (INamedTypeSymbol)property.Type;
        if (namedType.TypeArguments.Length == 0)
            return namedType.ToDisplayString();

        var itemType = namedType.TypeArguments[0];
        if (!itemType.IsComplexType() || itemType.Kind != SymbolKind.NamedType)
            return namedType.ToDisplayString();

        var itemNamedType = (INamedTypeSymbol)itemType;
        return $"{namedType.Name}<{itemNamedType.ContainingNamespace}.ReadOnly{itemNamedType.Name}>" +
               (property.Type.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "");
    }

    private static string GetArrayTypeString(IPropertySymbol property)
    {
        var arrayType = (IArrayTypeSymbol)property.Type;
        var elementType = arrayType.ElementType;

        if (!elementType.IsComplexType() || elementType.Kind != SymbolKind.NamedType)
            return arrayType.ToDisplayString();

        var elementNamedType = (INamedTypeSymbol)elementType;
        return $"{elementNamedType.ContainingNamespace}.ReadOnly{elementNamedType.Name}[]" +
               (property.Type.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "");
    }

    private static string GetPropertyAccessors(IPropertySymbol property)
    {
        if (!property.PreserveOriginalGetterSetter())
            return "get; set;";

        var getter = "get";
        var getterAccessibility = property.GetMethod!.DeclaredAccessibility.StringfyAccessibility();
        if (getterAccessibility != "public")
            getter = $"{getterAccessibility} get";

        var setter = "set";
        var setterAccessibility = property.SetMethod!.DeclaredAccessibility.StringfyAccessibility();
        if (setterAccessibility != "public")
            getter = $"{setterAccessibility} set";

        return $"{getter}; {setter}";
    }

    private static IEnumerable<string> GenerateAggregationProperties(IPropertySymbol property)
    {
        var aggregationAttributes = property.GetAttributes()
            .Where(a => a.AttributeClass != null && AggregationAttributes.Contains(a.AttributeClass.Name))
            .ToList();

        if (!aggregationAttributes.Any())
            return [];

        return aggregationAttributes
            .GroupBy(a => a.AttributeClass!.Name)
            .SelectMany(g => ProcessAttributeGroup(g, property));
    }

    private static IEnumerable<string> ProcessAttributeGroup(
        IGrouping<string, AttributeData> group,
        IPropertySymbol property)
    {
        var results = new List<string>();
        var attributeType = group.Key.Replace("Attribute", "");
        var autoNamedAttributes = group.Where(a =>
            string.IsNullOrEmpty(GetAttributeArgumentValue(a, "generatedPropertyName"))).ToList();

        for (int i = 0; i < autoNamedAttributes.Count; i++)
        {
            var attr = autoNamedAttributes[i];
            var suffix = property.Name + (autoNamedAttributes.Count > 1 ? $"_{i + 1}" : "");
            results.Add(CreateAggregationProperty(attr, attributeType, property, suffix));
        }

        foreach (var attr in group.Where(a =>
            !string.IsNullOrEmpty(GetAttributeArgumentValue(a, "generatedPropertyName"))))
        {
            results.Add(CreateAggregationProperty(attr, attributeType, property));
        }

        return results;
    }

    private static string CreateAggregationProperty(
        AttributeData attr,
        string attributeType,
        IPropertySymbol property,
        string? suffix = null)
    {
        var customName = GetAttributeArgumentValue(attr, "generatedPropertyName");
        var targetPropertyName = GetAttributeArgumentValue(attr, "aggregatePropertyName");
        var propertyName = customName ?? $"{attributeType}_{suffix ?? property.Name}";

        if (targetPropertyName != null && string.IsNullOrWhiteSpace(customName))
        {
            propertyName += $"_{targetPropertyName}";
        }

        var propertyType = GetAggregationPropertyType(attributeType, property, targetPropertyName);
        return $"public {propertyType} {propertyName} {{ get; set; }}";
    }

    private static string GetAggregationPropertyType(
        string attributeType,
        IPropertySymbol property,
        string? targetPropertyName)
    {
        if (attributeType == "Count") return "int";
        if (attributeType is "Min" or "Max") return GetTargetPropertyType(property, targetPropertyName) ?? "decimal";

        var baseType = GetTargetPropertyType(property, targetPropertyName) ?? "decimal";

        return attributeType switch
        {
            "Sum" when baseType is "int" or "short" or "byte" => "long",
            "Sum" when baseType == "long" => "decimal",
            "Average" when baseType == "float" => "double",
            "Average" when baseType is "int" or "short" or "byte" or "long" => "decimal",
            _ => baseType
        };
    }

    private static string? GetTargetPropertyType(IPropertySymbol property, string? targetPropertyName)
    {
        if (targetPropertyName == null ||
            property.Type is not INamedTypeSymbol { TypeArguments.Length: > 0 } namedType ||
            namedType.TypeArguments[0] is not INamedTypeSymbol itemType)
        {
            return null;
        }

        return itemType.GetMembers(targetPropertyName)
            .OfType<IPropertySymbol>()
            .FirstOrDefault()?
            .Type.ToDisplayString();
    }

    private static string? GetAttributeArgumentValue(AttributeData attr, string argumentName)
    {
        var namedArg = attr.NamedArguments.FirstOrDefault(a => a.Key == argumentName);
        if (namedArg.Value.Value != null) return namedArg.Value.Value.ToString();

        if (attr.AttributeConstructor == null) return null;

        var parameters = attr.AttributeConstructor.Parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].Name == argumentName && i < attr.ConstructorArguments.Length)
            {
                return attr.ConstructorArguments[i].Value?.ToString();
            }
        }

        return null;
    }

    private static IEnumerable<string> GetAppendedPropertiesFromAttributes(INamedTypeSymbol type)
    {
        var appendAttributes = type.GetAttributes()
            .Where(a => a.AttributeClass?.Name == "AppendPropertyToReadModelAttribute")
            .ToList();

        foreach (var attribute in appendAttributes)
        {
            if (attribute.ConstructorArguments.Length < 2) continue;

            var typeString = attribute.ConstructorArguments[0].Value?.ToString();
            var propertyName = attribute.ConstructorArguments[1].Value?.ToString();

            if (string.IsNullOrWhiteSpace(typeString) || string.IsNullOrWhiteSpace(propertyName))
                continue;

            if (!IsValidType(typeString, ValidPrimitiveTypes))
                continue;

            yield return $"public {typeString} {propertyName} {{ get; set; }}";
        }
    }

    private static bool IsValidType(string type, HashSet<string> validTypes)
    {
        if (validTypes.Contains(type))
            return true;

        // Handle nullable types (int?, string?)
        if (type.EndsWith("?") && validTypes.Contains(type.Substring(0, type.Length - 1)))
            return true;

        // Handle array types (int[], string[][])
        if (type.EndsWith("[]"))
            return IsValidType(type.Substring(0, type.Length - 2), validTypes) ||
                   IsValidGenericType(type.Substring(0, type.Length - 2));

        return IsValidGenericType(type);
    }

    private static bool IsValidGenericType(string type)
    {
        try
        {
            if (!type.Contains('<') || !type.EndsWith(">")) return false;
            return type.Split('<', '>', ',')
                .Select(p => p.Trim())
                .All(p => string.IsNullOrEmpty(p) || SyntaxFacts.IsValidIdentifier(p));
        }
        catch
        {
            return false;
        }
    }

    private static string ProcessReadModelAttributes(IPropertySymbol property)
    {
        var attributes = property.GetAttributes()
            .Where(a => a.AttributeClass?.Name == "AppendAttributeToReadModelAttribute")
            .Select(ProcessSingleReadModelAttribute)
            .Where(a => !string.IsNullOrEmpty(a));

        return string.Join("\r\n", attributes);
    }

    private static string ProcessSingleReadModelAttribute(AttributeData attribute)
    {
        // Early return if no constructor arguments
        if (attribute.ConstructorArguments.IsEmpty)
        {
            return string.Empty;
        }

        // Safely get attribute text with null-coalescing
        var attrText = attribute.ConstructorArguments[0].Value?.ToString() ?? string.Empty;

        // Handle simple attribute case (no parameters)
        if (!attrText.Contains('('))
        {
            return $"[{attrText}]";
        }

        // Parse complex attribute with parameters
        try
        {
            var openParenIndex = attrText.IndexOf('(');
            var closeParenIndex = attrText.LastIndexOf(')');

            // Validate parentheses positions
            if (openParenIndex <= 0 || closeParenIndex <= openParenIndex)
            {
                return $"[{attrText}]"; // Fallback for malformed syntax
            }

            // Extract components
            var attributeName = attrText.Substring(0, openParenIndex);
            var parameters = attrText.Substring(
                startIndex: openParenIndex + 1,
                length: closeParenIndex - openParenIndex - 1);

            // Normalize parameter quotes
            parameters = parameters.Replace("\"\"", "\"");

            return $"[{attributeName}({parameters})]";
        }
        catch
        {
            // Fallback to simple output if parsing fails
            return $"[{attrText}]";
        }
    }
}