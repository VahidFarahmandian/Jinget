namespace Jinget.SourceGenerator.Generators;

[Generator]
public class MainGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var generatorTypes = new List<Type>
        {
            typeof(ReadModelGenerator),
            typeof(WebAPIGenerator),
            typeof(ReadModelMappingConfigurationGenerator)
        };

        SourceGeneratorInitializer.InitializeGenerators(context, generatorTypes);
    }
}
