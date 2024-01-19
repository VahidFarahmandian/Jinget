namespace Jinget.Core.DiScanner.Internal;

internal class ImplementationTypeSelector(ITypeSourceSelector inner, IEnumerable<Type> types) : IImplementationTypeSelector, ISelector
{
    private ITypeSourceSelector Inner { get; } = inner;

    private IEnumerable<Type> Types { get; } = types;

    private List<ISelector> Selectors { get; } = [];

    public IServiceTypeSelector AddClasses() => AddClasses(publicOnly: true);

    public IServiceTypeSelector AddClasses(bool publicOnly)
    {
        var classes = GetNonAbstractClasses(publicOnly);

        return AddSelector(classes);
    }

    public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action) => AddClasses(action, publicOnly: false);

    public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly)
    {
        Preconditions.NotNull(action, nameof(action));

        var classes = GetNonAbstractClasses(publicOnly);

        var filter = new ImplementationTypeFilter(classes);

        action(filter);

        return AddSelector(filter.Types);
    }

    #region Chain Methods

    public IImplementationTypeSelector FromCallingAssembly() => Inner.FromCallingAssembly();

    public IImplementationTypeSelector FromExecutingAssembly() => Inner.FromExecutingAssembly();

    public IImplementationTypeSelector FromEntryAssembly() => Inner.FromEntryAssembly();

    public IImplementationTypeSelector FromApplicationDependencies() => Inner.FromApplicationDependencies();

    public IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate) => Inner.FromApplicationDependencies(predicate);

    public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly) => Inner.FromAssemblyDependencies(assembly);

    public IImplementationTypeSelector FromDependencyContext(DependencyContext context) => Inner.FromDependencyContext(context);

    public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate) => Inner.FromDependencyContext(context, predicate);

    public IImplementationTypeSelector FromAssemblyOf<T>() => Inner.FromAssemblyOf<T>();

    public IImplementationTypeSelector FromAssembliesOf(params Type[] types) => Inner.FromAssembliesOf(types);

    public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types) => Inner.FromAssembliesOf(types);

    public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies) => Inner.FromAssemblies(assemblies);

    public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies) => Inner.FromAssemblies(assemblies);

    #endregion

    void ISelector.Populate(IServiceCollection services, RegistrationStrategy? registrationStrategy)
    {
        if (Selectors.Count == 0)
        {
            AddClasses();
        }

        foreach (var selector in Selectors)
        {
            selector.Populate(services, registrationStrategy);
        }
    }

    private IServiceTypeSelector AddSelector(IEnumerable<Type> types)
    {
        var selector = new ServiceTypeSelector(this, types);

        Selectors.Add(selector);

        return selector;
    }

    private IEnumerable<Type> GetNonAbstractClasses(bool publicOnly) => Types.Where(t => t.IsNonAbstractClass(publicOnly));
}
