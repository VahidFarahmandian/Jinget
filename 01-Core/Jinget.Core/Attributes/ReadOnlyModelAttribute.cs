namespace Jinget.Core.Attributes;

/// <summary>
/// Used to mark readonly models used for CQRS(While working with IReadOnlyRepository)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ReadOnlyModelAttribute : Attribute
{
}
