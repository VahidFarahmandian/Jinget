﻿using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Generators;

[GeneratorOrder(Order = 2)]
public class ReadModelMappingConfigurationGenerator : IIncrementalGenerator
{
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration && classDeclaration.HasAttribute("GenerateReadModelMappingConfiguration");
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. Define data source (find all entity classes)
        var entityDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (c, _) => c.GetSemanticTargetForGeneration())
            .Where(static data => data != null)
            .Collect();

        // 2. Transform data (generate code for each entity)
        var compilationProvider = context.CompilationProvider.Combine(entityDeclarations);
        var transformedData = compilationProvider.Select((data, _) =>
        {
            var (compilation, entities) = data;
            return GenerateReadModelMappingCode(compilation, entities);
        });

        // 3. Add generated code to compilation
        context.RegisterSourceOutput(transformedData,
            static (spc, source) =>
            {
                foreach (var (HintName, Code) in source)
                {
                    spc.AddSource($"{HintName}.g.cs", SourceText.From(Code, Encoding.UTF8));
                }
            });
    }

    internal static ImmutableArray<(string HintName, string Code)> GenerateReadModelMappingCode(
        Compilation compilation,
        ImmutableArray<INamedTypeSymbol?> entityClasses)
    {
        if (compilation is null)
        {
            throw new ArgumentNullException(nameof(compilation));
        }

        return [.. entityClasses
            .Select(type =>
            {
                var originalClassName = type!.Name;
                var newClassName = $"ReadOnly{originalClassName}";

                // Find the WriteModelMappingConfiguration type 
                var mappingClass = compilation.GetTypeByMetadataName($"{type.ContainingNamespace}.{originalClassName}")
                ?? throw new InvalidOperationException($"Could not find the MappingConfiguration type for {originalClassName}. " +
                                                        $"Expected type name: {type.ContainingNamespace}.MappingConfigurations.{originalClassName}");

                // Ensure the mapping class is part of the current compilation
                if (!mappingClass.DeclaringSyntaxReferences.Any())
                {
                    throw new InvalidOperationException($"The type {mappingClass.Name} is not part of the current compilation.");
                }

                // Get the WriteModelMappingConfiguration.Configure method
                var mappingConfigureMethod = mappingClass.GetMembers().SingleOrDefault(m => m.Name == "Configure") as IMethodSymbol
                ?? throw new InvalidOperationException($"Could not find the 'Configure' method in {mappingClass.Name}");

                // Get the syntax node for the Configure method
                var mappingConfigureMethodDeclaration = mappingConfigureMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax
                ?? throw new InvalidOperationException($"Could not retrieve the syntax node for the 'Configure' method in {mappingClass.Name}");

                // Get the method body 
                var originalMappingConfigureMethodBody = mappingConfigureMethodDeclaration.Body;

                // Get Base Type of WriteModelMappingConfiguration
                var baseType =mappingClass?.BaseType?.Name=="Object" ?mappingClass.Interfaces.First(): mappingClass?.BaseType;

                // Get generic type arguments of the base type
                var modelName = type.GetAttributeNamedArgument<string>(compilation, "GenerateReadModelMappingConfiguration", "Model", "");
                modelName = string.IsNullOrWhiteSpace(modelName) ? baseType!.TypeArguments[0].Name : modelName;
                var originalModelName = compilation.FindTypeInReferencedAssemblies(modelName);

                var newModelName = $"ReadOnly{originalModelName!.Name}";
                var fullyQualifiedNewModelName = $"{originalModelName.ContainingNamespace}.{newModelName}";

                string baseTypeName="";

                string otherGenericTypes="";
                if(baseType?.TypeArguments.Count()>1)
                {
                    otherGenericTypes = string.Join(",", baseType.TypeArguments.Skip(1).Select(x=>x.ToDisplayString()));
                    baseTypeName = $"{baseType.Name}<{fullyQualifiedNewModelName},{otherGenericTypes}>";
                }
                else if(baseType!=null){
                    baseTypeName = $"{baseType.Name}<{fullyQualifiedNewModelName}>";
                }

                var ignoredProperties = GenerateIgnoredProperties(compilation, originalModelName).ToList();

                var newMappingConfigureMethodBody = originalMappingConfigureMethodBody!
                //add builder.Ignore() for properties which are appended to readonly model(and does not exists in model)
                .AddIgnoreMappingStatements(ignoredProperties)

                //replace //ignorethisline comment's statements
                .ReplaceIgnoredStatements()
                
                //remove all comments from method body
                .RemoveAllComments()

                //replace generic types in method bodies like: builder.MapColumnsByName<SampleModel, Guid>();
                .ReplaceGenericArgumentsInMethodBody(compilation)

                //replace model names in method body like nameod(SampleModel)
                .ReplaceModelNamesInMethodBody(compilation)

                //replace model names in lambda expressions like HasForeignKey((SampleModel x) => x.Id)
                .ReplaceModelNamesInLambda(compilation)

                //remove extra whitespaces from method body
                .RemoveExtraWhitespace();

                // Extract the method body as a string and remove the outer braces
                var newMappingConfigureMethodBodyString = RemoveExtraBrackets(newMappingConfigureMethodBody?.ToFullString()!);

                var usingsList=mappingClass.GetUsings();
                string usings="";
                if(usingsList.Any()){
                    if(usingsList.Count==1)
                        usings = $"using {mappingClass.GetUsings().First()};";
                    else
                        usings = string.Join(";\r\n", mappingClass.GetUsings().Select(x=>$"using {x}"))+ ";";
                }

                string overrideString = mappingClass?.GetMembers().OfType<IMethodSymbol>().Where(x=>x.Name=="Configure" && x.IsOverride).FirstOrDefault()!=null?" override ":" ";

                var classContent =
$@"{usings}
namespace {type.ContainingNamespace};
public class {newClassName}: {baseTypeName}
{{
    public{overrideString}void Configure(EntityTypeBuilder<{fullyQualifiedNewModelName}> builder)
    {{
        {newMappingConfigureMethodBodyString}
    }}
}}".Trim();
                return (HintName: newClassName, Code: classContent);
            })];
    }

    private static string RemoveExtraBrackets(string input)
    {
        if (input == null)
        {
            return "";
        }
        input = input.Trim();
        if (input.StartsWith("{"))
        {
            input = input.Substring(1).Trim();
        }
        if (input.EndsWith("}"))
        {
            input = input.Substring(0, input.Length - 1).Trim();
        }
        return input;
    }

    private static List<string> GenerateIgnoredProperties(Compilation compilation, INamedTypeSymbol type)
    {
        List<string> ignoreMappingProperties = [];

        //class level custom properties
        foreach (var attr in type.GetAttributes().Where(a => a.AttributeClass?.Name == "AppendPropertyToReadModelAttribute"))
        {
            var generatePropertyName = attr.ConstructorArguments[1].Value;
            var isIgnoreMapping = Convert.ToBoolean(attr.ConstructorArguments[2].Value);
            if (isIgnoreMapping && generatePropertyName != null)
            {
                ignoreMappingProperties.Add(generatePropertyName.ToString());
            }
        }


        foreach (var prop in type.GetMembers().Where(m => m.Kind == SymbolKind.Property).OfType<IPropertySymbol>())
        {
            //property level custom properties via attributes(count, sum, min, max, average)
            foreach (var attr in prop.GetAttributes().Where(a => a.AttributeClass?.BaseType != null && a.AttributeClass.BaseType.Name == "BaseAggregationAttribute"))
            {
                var generatePropertyName = attr.ConstructorArguments[0].Value;
                var isIgnoreMapping = Convert.ToBoolean(attr.ConstructorArguments[1].Value);
                if (isIgnoreMapping && generatePropertyName != null)
                {
                    ignoreMappingProperties.Add(generatePropertyName.ToString());
                }
            }

            //properties marked with [IgnoreMapping] attribute
            foreach (var attr in prop.GetAttributes().Where(a => a.AttributeClass?.Name == "IgnoreMappingAttribute"))
            {
                ignoreMappingProperties.Add(prop.Name);
            }
        }

        return ignoreMappingProperties;
    }
}