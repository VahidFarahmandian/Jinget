using System.Collections.Concurrent;

namespace Jinget.Core.Types.ValueObject;

public abstract class JingetValueObject : IEquatable<JingetValueObject>
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    protected virtual IEnumerable<object?> YieldProperties()
    {
        var properties = PropertyCache.GetOrAdd(
            GetType(),
            static t => [.. t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                         .Where(p => p.CanRead)
                         .OrderBy(p => p.Name)]);

        foreach (var property in properties)
            yield return property.GetValue(this);
    }

    protected virtual void Validate()
    {
    }

    public override bool Equals(object? obj)
        => Equals(obj as JingetValueObject);

    public bool Equals(JingetValueObject? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        if (GetType() != other.GetType())
            return false;

        using var left = YieldProperties().GetEnumerator();
        using var right = other.YieldProperties().GetEnumerator();

        while (true)
        {
            var hasLeft = left.MoveNext();
            var hasRight = right.MoveNext();

            if (hasLeft != hasRight)
                return false;

            if (!hasLeft)
                break;

            if (!AreEqual(left.Current, right.Current))
                return false;
        }

        return true;
    }

    private static bool AreEqual(object? left, object? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        if (left is string || right is string)
            return Equals(left, right);

        if (left is IEnumerable leftEnumerable &&
            right is IEnumerable rightEnumerable)
        {
            var leftEnumerator = leftEnumerable.GetEnumerator();
            var rightEnumerator = rightEnumerable.GetEnumerator();

            while (true)
            {
                var hasLeft = leftEnumerator.MoveNext();
                var hasRight = rightEnumerator.MoveNext();

                if (hasLeft != hasRight)
                    return false;

                if (!hasLeft)
                    break;

                if (!AreEqual(leftEnumerator.Current, rightEnumerator.Current))
                    return false;
            }

            return true;
        }

        return Equals(left, right);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var property in YieldProperties())
        {
            AddHash(ref hash, property);
        }

        return hash.ToHashCode();
    }

    private static void AddHash(ref HashCode hash, object? value)
    {
        if (value is null)
        {
            hash.Add(0);
            return;
        }

        if (value is string)
        {
            hash.Add(value);
            return;
        }

        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                AddHash(ref hash, item);
            }

            return;
        }

        hash.Add(value);
    }

    public static bool operator ==(JingetValueObject? left, JingetValueObject? right)
        => Equals(left, right);

    public static bool operator !=(JingetValueObject? left, JingetValueObject? right)
        => !Equals(left, right);
}