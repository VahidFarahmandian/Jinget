namespace Jinget.Core.DiScanner.Contracts;

public interface ITypeSelector : IFluentInterface
{
    /// <summary>
    /// Will scan the types <see cref="Type"/> in <paramref name="types"/>.
    /// </summary>
    /// <param name="types">The types in which assemblies that should be scanned.</param>
    /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
    IServiceTypeSelector AddTypes(params Type[] types);

    /// <summary>
    /// Will scan the types <see cref="Type"/> in <paramref name="types"/>.
    /// </summary>
    /// <param name="types">The types in which assemblies that should be scanned.</param>
    /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
    IServiceTypeSelector AddTypes(IEnumerable<Type> types);
}
