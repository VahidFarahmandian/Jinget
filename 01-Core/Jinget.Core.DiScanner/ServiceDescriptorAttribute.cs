namespace Jinget.Core.DiScanner;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ServiceDescriptorAttribute(Type? serviceType, ServiceLifetime lifetime) : Attribute
{
    public ServiceDescriptorAttribute() : this(null) { }

    public ServiceDescriptorAttribute(Type? serviceType) : this(serviceType, ServiceLifetime.Transient) { }

    public Type? ServiceType { get; } = serviceType;

    public ServiceLifetime Lifetime { get; } = lifetime;

    public IEnumerable<Type> GetServiceTypes(Type fallbackType)
    {
        if (ServiceType is null)
        {
            yield return fallbackType;

            var fallbackTypes = fallbackType.GetBaseTypes();

            foreach (var type in fallbackTypes)
            {
                if (type == typeof(object))
                {
                    continue;
                }

                yield return type;
            }

            yield break;
        }

        if (!fallbackType.IsBasedOn(ServiceType))
        {
            throw new InvalidOperationException($@"Type ""{fallbackType.ToFriendlyName()}"" is not assignable to ""{ServiceType.ToFriendlyName()}"".");
        }

        yield return ServiceType;
    }
}
