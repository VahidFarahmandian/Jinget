namespace Jinget.Core.Types.ValueObject;

public abstract class JingetValueObject : IEquatable<JingetValueObject>
{
    /// <summary>
    /// Gets the properties values of the object for equality comparison.
    /// </summary>
    /// <example>
    ///protected override IEnumerable<object> GetAtomicValues()
    ///{
    ///    yield return Property1;
    ///    yield return Propert2;
    ///}
    /// </example>
    /// <returns>An enumerable of objects that represent the value object's values.</returns>
    protected virtual IEnumerable<object> YieldProperties()
    {
        var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var readableProperties = properties.Where(p => p.CanRead)?.OrderBy(p => p.Name);
        return readableProperties?.Select(p => p.GetValue(this))!;
    }

    /// <summary>
    /// Validates the value object's state.
    /// </summary>
    protected virtual void Validate() { }

    public override bool Equals(object? obj)
    {
        return Equals(obj as JingetValueObject);
    }

    public bool Equals(JingetValueObject? other)
    {
        if (other is null || other.GetType() != GetType())
            return false;

        return YieldProperties().SequenceEqual(other.YieldProperties());
    }

    public override int GetHashCode()
    {
        return YieldProperties()
            .Aggregate(
                default(int),
                (hashcode, value) =>
                    HashCode.Combine(hashcode, value?.GetHashCode() ?? 0)
            );
    }

    public static bool operator ==(JingetValueObject left, JingetValueObject right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(JingetValueObject left, JingetValueObject right)
    {
        return !(left == right);
    }

}
