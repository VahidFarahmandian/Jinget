namespace Jinget.Core.DiScanner;

public class TypeSourceSelector : ITypeSourceSelector, ISelector
{
    private static Assembly EntryAssembly => Assembly.GetEntryAssembly()
        ?? throw new InvalidOperationException("Could not get entry assembly.");

    private List<ISelector> Selectors { get; } = [];

    /// <inheritdoc />
    public IImplementationTypeSelector FromAssemblyOf<T>() => InternalFromAssembliesOf([typeof(T)]);

    public IImplementationTypeSelector FromCallingAssembly() => FromAssemblies(Assembly.GetCallingAssembly());

    public IImplementationTypeSelector FromExecutingAssembly() => FromAssemblies(Assembly.GetExecutingAssembly());

    public IImplementationTypeSelector FromEntryAssembly() => FromAssemblies(EntryAssembly);

    public IImplementationTypeSelector FromApplicationDependencies() => FromApplicationDependencies(_ => true);

    public IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate)
    {
        try
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return FromDependencyContext(DependencyContext.Default, predicate);
#pragma warning restore CS8604 // Possible null reference argument.
        }
        catch
        {
            // Something went wrong when loading the DependencyContext, fall
            // back to loading all referenced assemblies of the entry assembly...
            return FromAssemblyDependencies(EntryAssembly);
        }
    }

    public IImplementationTypeSelector FromDependencyContext(DependencyContext context) => FromDependencyContext(context, _ => true);

    public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate)
    {
        Preconditions.NotNull(context, nameof(context));
        Preconditions.NotNull(predicate, nameof(predicate));

        var assemblyNames = context.RuntimeLibraries
            .SelectMany(library => library.GetDefaultAssemblyNames(context));

        var assemblies = LoadAssemblies(assemblyNames);

        return InternalFromAssemblies(assemblies.Where(predicate));
    }

    public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly)
    {
        Preconditions.NotNull(assembly, nameof(assembly));

        var assemblies = new List<Assembly> { assembly };

        try
        {
            var dependencyNames = assembly.GetReferencedAssemblies();

            assemblies.AddRange(LoadAssemblies(dependencyNames));

            return InternalFromAssemblies(assemblies);
        }
        catch
        {
            return InternalFromAssemblies(assemblies);
        }
    }

    public IImplementationTypeSelector FromAssembliesOf(params Type[] types)
    {
        Preconditions.NotNull(types, nameof(types));

        return InternalFromAssembliesOf(types);
    }

    public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types)
    {
        Preconditions.NotNull(types, nameof(types));

        return InternalFromAssembliesOf(types);
    }

    public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies)
    {
        Preconditions.NotNull(assemblies, nameof(assemblies));

        return InternalFromAssemblies(assemblies);
    }

    public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies)
    {
        Preconditions.NotNull(assemblies, nameof(assemblies));

        return InternalFromAssemblies(assemblies);
    }

    public IServiceTypeSelector AddTypes(params Type[] types)
    {
        Preconditions.NotNull(types, nameof(types));

        var selector = new ImplementationTypeSelector(this, types);

        Selectors.Add(selector);

        return selector.AddClasses();
    }

    public IServiceTypeSelector AddTypes(IEnumerable<Type> types)
    {
        Preconditions.NotNull(types, nameof(types));

        var selector = new ImplementationTypeSelector(this, types);

        Selectors.Add(selector);

        return selector.AddClasses();
    }

    public void Populate(IServiceCollection services, RegistrationStrategy? registrationStrategy)
    {
        foreach (var selector in Selectors)
        {
            selector.Populate(services, registrationStrategy);
        }
    }

    private IImplementationTypeSelector InternalFromAssembliesOf(IEnumerable<Type> types) => InternalFromAssemblies(types.Select(t => t.Assembly));

    private IImplementationTypeSelector InternalFromAssemblies(IEnumerable<Assembly> assemblies) => AddSelector(assemblies.SelectMany(asm => asm.DefinedTypes).Select(x => x.AsType()));

    private static IEnumerable<Assembly> LoadAssemblies(IEnumerable<AssemblyName> assemblyNames)
    {
        var assemblies = new List<Assembly>();

        foreach (var assemblyName in assemblyNames)
        {
            try
            {
                // Try to load the referenced assembly...
                assemblies.Add(Assembly.Load(assemblyName));
            }
            catch
            {
                // Failed to load assembly. Skip it.
            }
        }

        return assemblies;
    }

    private IImplementationTypeSelector AddSelector(IEnumerable<Type> types)
    {
        var selector = new ImplementationTypeSelector(this, types);

        Selectors.Add(selector);

        return selector;
    }
}
