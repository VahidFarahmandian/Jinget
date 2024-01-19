namespace Jinget.Core.DiScanner.Exceptions;

public class DuplicateTypeRegistrationException(Type serviceType) :
    InvalidOperationException($"A service of type '{serviceType.ToFriendlyName()}' has already been registered.")
{
    public Type ServiceType { get; } = serviceType;
}
