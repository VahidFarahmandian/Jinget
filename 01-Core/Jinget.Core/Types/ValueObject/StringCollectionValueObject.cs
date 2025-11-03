using System.Text.Json.Serialization;

namespace Jinget.Core.Types.ValueObject;

public class StringCollectionValueObject : JingetValueObject
{
    private StringCollectionValueObject()
    {

    }

    [JsonConstructor]
    public StringCollectionValueObject(ICollection<string> values) => Values = values;

    public ICollection<string> Values { get; protected set; }

}
