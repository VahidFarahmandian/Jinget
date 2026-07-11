using Jinget.Core.Types.ValueObject;

namespace Jinget.Core.Tests._BaseData;

internal sealed class TestValueObject : JingetValueObject
{
    public string Name { get; }
    public int Age { get; }
    public List<string> Tags { get; }

    public TestValueObject(string name, int age, params string[] tags)
    {
        Name = name;
        Age = age;
        Tags = tags.ToList();
    }
}

internal sealed class AnotherValueObject : JingetValueObject
{
    public string Name { get; }

    public AnotherValueObject(string name)
    {
        Name = name;
    }
}

internal sealed class NullableValueObject : JingetValueObject
{
    public string? Name { get; }

    public NullableValueObject(string? name)
    {
        Name = name;
    }
}

internal sealed class NullableCollectionValueObject : JingetValueObject
{
    public List<string>? Values { get; }

    public NullableCollectionValueObject(List<string>? values)
    {
        Values = values;
    }
}

internal sealed class AddressValueObject : JingetValueObject
{
    public string City { get; }

    public AddressValueObject(string city)
    {
        City = city;
    }
}

internal sealed class PersonValueObject : JingetValueObject
{
    public AddressValueObject Address { get; }

    public PersonValueObject(AddressValueObject address)
    {
        Address = address;
    }
}