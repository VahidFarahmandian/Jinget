using System.Reflection;

namespace Jinget.SourceGenerator;

public static class SourceGeneratorInitializer
{
    public static void InitializeGenerators(IncrementalGeneratorInitializationContext context, IEnumerable<Type> generatorTypes)
    {
        var initializedGenerators = new HashSet<Type>();

        var orderedGenerators = generatorTypes
            .Where(t => typeof(IIncrementalGenerator).IsAssignableFrom(t))
            .OrderBy(t => t.GetCustomAttribute<GeneratorOrderAttribute>()?.Order ?? int.MaxValue)
            .Select(t => (IIncrementalGenerator)Activator.CreateInstance(t));

        foreach (var generator in orderedGenerators)
        {
            var generatorType = generator.GetType();
            if (!initializedGenerators.Contains(generatorType))
            {
                generator.Initialize(context);
                initializedGenerators.Add(generatorType);
            }
        }
    }
}
