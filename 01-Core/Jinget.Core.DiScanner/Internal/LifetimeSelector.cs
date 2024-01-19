namespace Jinget.Core.DiScanner.Internal;

internal sealed class LifetimeSelector(ServiceTypeSelector inner, IEnumerable<TypeMap> typeMaps, IEnumerable<TypeFactoryMap> typeFactoryMaps) : ILifetimeSelector, ISelector
{
    private ServiceTypeSelector Inner { get; } = inner;

    private IEnumerable<TypeMap> TypeMaps { get; } = typeMaps;

    private IEnumerable<TypeFactoryMap> TypeFactoryMaps { get; } = typeFactoryMaps;

    public ServiceLifetime? Lifetime { get; set; }

    public IImplementationTypeSelector WithSingletonLifetime() => WithLifetime(ServiceLifetime.Singleton);

    public IImplementationTypeSelector WithScopedLifetime() => WithLifetime(ServiceLifetime.Scoped);

    public IImplementationTypeSelector WithTransientLifetime() => WithLifetime(ServiceLifetime.Transient);

    public IImplementationTypeSelector WithLifetime(ServiceLifetime lifetime)
    {
        Preconditions.IsDefined(lifetime, nameof(lifetime));

        Inner.PropagateLifetime(lifetime);

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

    public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action) => Inner.AddClasses(action);

    public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly) => Inner.AddClasses(action, publicOnly);

    public ILifetimeSelector AsSelf() => Inner.AsSelf();

    public ILifetimeSelector As<T>() => Inner.As<T>();

    public ILifetimeSelector As(params Type[] types) => Inner.As(types);

    public ILifetimeSelector As(IEnumerable<Type> types) => Inner.As(types);

    public ILifetimeSelector AsImplementedInterfaces() => Inner.AsImplementedInterfaces();

    public ILifetimeSelector AsSelfWithInterfaces() => Inner.AsSelfWithInterfaces();

    public ILifetimeSelector AsMatchingInterface() => Inner.AsMatchingInterface();

    public ILifetimeSelector AsMatchingInterface(Action<Type, IImplementationTypeFilter> action) => Inner.AsMatchingInterface(action);

    public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector) => Inner.As(selector);

    public IImplementationTypeSelector UsingAttributes() => Inner.UsingAttributes();

    public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy) => Inner.UsingRegistrationStrategy(registrationStrategy);

    #endregion

    void ISelector.Populate(IServiceCollection services, RegistrationStrategy? strategy)
    {
        strategy ??= RegistrationStrategy.Append;

        var lifetime = Lifetime ?? ServiceLifetime.Transient;

        foreach (var typeMap in TypeMaps)
        {
            foreach (var serviceType in typeMap.ServiceTypes)
            {
                var implementationType = typeMap.ImplementationType;

                if (!implementationType.IsBasedOn(serviceType))
                {
                    throw new InvalidOperationException($@"Type ""{implementationType.ToFriendlyName()}"" is not assignable to ""${serviceType.ToFriendlyName()}"".");
                }

                var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);

                strategy.Apply(services, descriptor);
            }
        }

        foreach (var typeFactoryMap in TypeFactoryMaps)
        {
            foreach (var serviceType in typeFactoryMap.ServiceTypes)
            {
                var descriptor = new ServiceDescriptor(serviceType, typeFactoryMap.ImplementationFactory, lifetime);

                strategy.Apply(services, descriptor);
            }
        }
    }
}
