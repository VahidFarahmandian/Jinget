namespace Jinget.Core.Utilities.Enum;

public static class DbTypeUtility
{
    private static readonly Dictionary<Type, DbType> typeMap = new()
    {
        [typeof(byte)] = DbType.Byte,
        [typeof(sbyte)] = DbType.SByte,
        [typeof(short)] = DbType.Int16,
        [typeof(ushort)] = DbType.UInt16,
        [typeof(int)] = DbType.Int32,
        [typeof(uint)] = DbType.UInt32,
        [typeof(long)] = DbType.Int64,
        [typeof(ulong)] = DbType.UInt64,
        [typeof(float)] = DbType.Single,
        [typeof(double)] = DbType.Double,
        [typeof(decimal)] = DbType.Decimal,
        [typeof(bool)] = DbType.Boolean,
        [typeof(string)] = DbType.String,
        [typeof(char)] = DbType.StringFixedLength,
        [typeof(Guid)] = DbType.Guid,
        [typeof(DateTime)] = DbType.DateTime,
        [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
        [typeof(byte[])] = DbType.Binary,
        [typeof(byte?)] = DbType.Byte,
        [typeof(sbyte?)] = DbType.SByte,
        [typeof(short?)] = DbType.Int16,
        [typeof(ushort?)] = DbType.UInt16,
        [typeof(int?)] = DbType.Int32,
        [typeof(uint?)] = DbType.UInt32,
        [typeof(long?)] = DbType.Int64,
        [typeof(ulong?)] = DbType.UInt64,
        [typeof(float?)] = DbType.Single,
        [typeof(double?)] = DbType.Double,
        [typeof(decimal?)] = DbType.Decimal,
        [typeof(bool?)] = DbType.Boolean,
        [typeof(char?)] = DbType.StringFixedLength,
        [typeof(Guid?)] = DbType.Guid,
        [typeof(DateTime?)] = DbType.DateTime,
        [typeof(DateTimeOffset?)] = DbType.DateTimeOffset
    };

    /// <summary>
    /// Get corresponding <seealso cref="DbType"/> of the given <seealso cref="Type"/>
    /// </summary>
    public static DbType GetDbType(Type givenType)
    {
        givenType = Nullable.GetUnderlyingType(givenType) ?? givenType;

        if (typeMap.TryGetValue(givenType, out DbType value))
        {
            return value;
        }

        throw new ArgumentException($"Jinget Says: {givenType.FullName} is not a supported .NET class");
    }

    /// <summary>
    /// Get corresponding <seealso cref="DbType"/> of the given <typeparamref name="T"/> generic type
    /// </summary>
    public static DbType GetDbType<T>() => GetDbType(typeof(T));
}
