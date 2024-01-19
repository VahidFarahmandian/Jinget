namespace Jinget.Core.DiScanner.Internal;

internal readonly struct TypeFactoryMap(Func<IServiceProvider, object> implementationFactory, IEnumerable<Type> serviceTypes)
{
    public Func<IServiceProvider, object> ImplementationFactory { get; } = implementationFactory;

    public IEnumerable<Type> ServiceTypes { get; } = serviceTypes;
}
