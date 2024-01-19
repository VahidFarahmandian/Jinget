namespace Jinget.Core.DiScanner.Internal;

internal readonly struct TypeMap(Type implementationType, IEnumerable<Type> serviceTypes)
{
    public Type ImplementationType { get; } = implementationType;

    public IEnumerable<Type> ServiceTypes { get; } = serviceTypes;
}
