namespace Jinget.Core.DiScanner.Internal;

internal class ServiceTypeSelector(IImplementationTypeSelector inner, IEnumerable<Type> types) : IServiceTypeSelector, ISelector
{
    private IImplementationTypeSelector Inner { get; } = inner;

    private IEnumerable<Type> Types { get; } = types;

    private List<ISelector> Selectors { get; } = [];

    private RegistrationStrategy? RegistrationStrategy { get; set; }

    public ILifetimeSelector AsSelf() => As(t => [t]);

    public ILifetimeSelector As<T>() => As(typeof(T));

    public ILifetimeSelector As(params Type[] types)
    {
        Preconditions.NotNull(types, nameof(types));

        return As(types.AsEnumerable());
    }

    public ILifetimeSelector As(IEnumerable<Type> types)
    {
        Preconditions.NotNull(types, nameof(types));

        return AddSelector(Types.Select(t => new TypeMap(t, types)), []);
    }

    public ILifetimeSelector AsImplementedInterfaces() => As(t => t.GetInterfaces()
                                                                   .Where(x => x.HasMatchingGenericArity(t))
                                                                   .Select(x => x.GetRegistrationType(t)));

    public ILifetimeSelector AsSelfWithInterfaces()
    {
        IEnumerable<Type> Selector(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                // This prevents trying to register open generic types
                // with an ImplementationFactory, which is unsupported.
                return [];
            }

            return type.GetInterfaces()
                .Where(x => x.HasMatchingGenericArity(type))
                .Select(x => x.GetRegistrationType(type));
        }

        return AddSelector(
            Types.Select(t => new TypeMap(t, [t])),
            Types.Select(t => new TypeFactoryMap(x => x.GetRequiredService(t), Selector(t))));
    }

    public ILifetimeSelector AsMatchingInterface() => AsMatchingInterface(null);

    public ILifetimeSelector AsMatchingInterface(Action<Type, IImplementationTypeFilter>? action) => As(t => t.FindMatchingInterface(action));

    public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector)
    {
        Preconditions.NotNull(selector, nameof(selector));

        return AddSelector(Types.Select(t => new TypeMap(t, selector(t))), []);
    }

    public IImplementationTypeSelector UsingAttributes()
    {
        var selector = new AttributeSelector(Types);

        Selectors.Add(selector);

        return this;
    }

    public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy)
    {
        Preconditions.NotNull(registrationStrategy, nameof(registrationStrategy));

        RegistrationStrategy = registrationStrategy;
        return this;
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

    public IServiceTypeSelector AddClasses() => Inner.AddClasses();

    public IServiceTypeSelector AddClasses(bool publicOnly) => Inner.AddClasses(publicOnly);

    public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action)
    {
        var t = Inner.AddClasses(action);
        return t;
    }

    public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly) => Inner.AddClasses(action, publicOnly);

    #endregion

    internal void PropagateLifetime(ServiceLifetime lifetime)
    {
        foreach (var selector in Selectors.OfType<LifetimeSelector>())
        {
            selector.Lifetime = lifetime;
        }
    }

    void ISelector.Populate(IServiceCollection services, RegistrationStrategy? registrationStrategy)
    {
        if (Selectors.Count == 0)
        {
            AsSelf();
        }

        var strategy = RegistrationStrategy ?? registrationStrategy;

        foreach (var selector in Selectors)
        {
            selector.Populate(services, strategy);
        }
    }

    private ILifetimeSelector AddSelector(IEnumerable<TypeMap> types, IEnumerable<TypeFactoryMap> factories)
    {
        var selector = new LifetimeSelector(this, types, factories);

        Selectors.Add(selector);

        return selector;
    }
}
