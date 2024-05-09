using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace Jinget.Core.Utilities.Json;

/// <summary>
/// ignore the given properties, while serializing an object
/// </summary>
public class IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore) : DefaultContractResolver
{
    private readonly HashSet<string> _ignoreProps = new(propNamesToIgnore);

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);
        if (!string.IsNullOrWhiteSpace(property.PropertyName) &&
            _ignoreProps.Contains(property.PropertyName))
        {
            property.ShouldSerialize = _ => false;
        }
        return property;
    }
}
