namespace Jinget.Core.DiScanner.Exceptions;

public class MissingTypeRegistrationException(Type serviceType) :
    InvalidOperationException($"Could not find any registered services for type '{serviceType.ToFriendlyName()}'.")
{
    public Type ServiceType { get; } = serviceType;
}
