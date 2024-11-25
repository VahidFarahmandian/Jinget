namespace Jinget.Core.Attributes;

/// <summary>
/// name used as cache key. this name should be unique among models and its corresponding viewmodel to correct cache invalidation
/// </summary>
/// <param name="identifier"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CacheTypeIdentifier(string identifier) : Attribute
{
    public string Identifier { get; set; } = identifier;
}
