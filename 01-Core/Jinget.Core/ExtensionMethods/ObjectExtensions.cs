namespace Jinget.Core.ExtensionMethods;

public static class ObjectExtensions
{
    public class Options(bool ignoreNull = true, bool ignoreExpressions = true)
    {
        public bool IgnoreNull { get; set; } = ignoreNull;
        public bool IgnoreExpressions { get; set; } = ignoreExpressions;
        public bool IgnoreExpr2SQLPagings { get; set; } = true;
        public bool IgnoreExpr2SQLOrderBys { get; set; } = true;
    }


    /// <summary>
    /// Check if given type is numeric or not
    /// </summary>
    public static bool IsNumericType(this object o) => Type.GetTypeCode(o.GetType()) switch
    {
        TypeCode.Byte or
        TypeCode.SByte or
        TypeCode.UInt16 or
        TypeCode.UInt32 or
        TypeCode.UInt64 or
        TypeCode.Int16 or
        TypeCode.Int32 or
        TypeCode.Int64 or
        TypeCode.Decimal or
        TypeCode.Double or
        TypeCode.Single => true,
        _ => false,
    };

    /// <summary>
    /// Convert source object to dictionary(key=property name, value=property value)
    /// </summary>
    public static Dictionary<string, object?> ToDictionary(this object? source, Options? options = null)
    {
        if (source == null)
            return [];
        options ??= new Options();

        Dictionary<string, object?> result = [];

        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
        {
            object? value = property.GetValue(source);

            if (options.IgnoreNull && value == null)
                continue;
            else if (options.IgnoreExpressions && value is Expression)
                continue;
            else if (options.IgnoreExpr2SQLPagings && value is Paging)
                continue;
            else if (options.IgnoreExpr2SQLOrderBys)
            {
                if (value is List<OrderBy>)
                    continue;
                else if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GenericTypeArguments[0].IsGenericType &&
                    property.PropertyType.GenericTypeArguments[0].GetGenericTypeDefinition() == typeof(OrderBy<>))
                    continue;
            }
            string key = property.Name;
            if (!result.ContainsKey(key))
            {
                result.Add(key, value);
            }
        }

        return result;
    }

    /// <summary>
    /// Get the value of the given property
    /// </summary>
    public static object? GetValue(this object obj, string propertyName)
    {
        if (obj == null)
            return null;
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }

    /// <summary>
    /// Convert two unrelated objects to each other.
    /// </summary>
    public static T? ToType<T>(this object obj, bool suppressError = false) where T : class
    {
        Type type = typeof(T);

        var newObj = type.InvokeDefaultConstructor<T>();

        if (obj == null)
            return newObj;

        foreach (PropertyInfo pi in obj.GetType().GetProperties().Where(x => x.Name != "Empty" && x.CanWrite))
        {
            var property = type.GetProperty(pi.Name);

            if (property == null)
            {
                if (suppressError)
                    continue;
                throw new JingetException($"Jinget Says: Model {typeof(T).Name} does not contain property with the name {pi.Name}", 1000);
            }

            if (pi.PropertyType.IsValueType || pi.PropertyType == typeof(string))
            {
                property.SetValue(newObj, pi.GetValue(obj, null));
            }
            else
            {
                var value = pi.GetValue(obj);

                if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>))
                {
                    var referenceTypeValue = typeof(ObjectExtensions).Call(
                        name: nameof(ToType),
                        null,
                        parameterValues: [value],
                        generics: property.PropertyType);

                    property.SetValue(newObj, referenceTypeValue);
                }
                else
                {
                    if (value == null)
                        continue;

                    foreach (var
                        item in (IEnumerable)value)
                    {
                        var referenceTypeValue = typeof(ObjectExtensions).Call(
                            name: nameof(ToType),
                            null,
                            parameterValues: [item],
                            generics: property.PropertyType.GetGenericArguments()[0]);

                        if (property.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>))
                        {
                            property.SetValue(newObj, referenceTypeValue);
                        }
                        else
                        {
                            var field = type.GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

                            if (field == null)
                            {
                                field = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                    .FirstOrDefault(x => string.Equals(x.Name, "_" + property.Name,
                                        StringComparison.OrdinalIgnoreCase));

                                if (field == null)
                                {
                                    throw new JingetException($"Jinget Says: Unable to find backing field for Property '{property.Name}'", 1000);
                                }
                            }

                            if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                            {
                                if (field.GetValue(newObj) == null)//init the list
                                {
                                    var instance = typeof(List<>)
                                        .MakeGenericType(property.PropertyType.GetGenericArguments()[0])
                                        .GetConstructor(Type.EmptyTypes)
                                        ?.Invoke(null);

                                    field.SetValue(newObj, instance);
                                }

                                field.FieldType.GetMethod("Add")?.Invoke(field.GetValue(newObj), [referenceTypeValue]);
                            }
                            else
                            {
                                field.SetValue(newObj, referenceTypeValue);
                            }
                        }
                    }
                }
            }
        }
        return newObj;
    }

    /// <summary>
    /// check if two objects(from same/different types) have exactly same structure and values
    /// </summary>
    public static bool HasSameValuesAs(this object source, object target)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        return JsonConvert.SerializeObject(source, settings) == JsonConvert.SerializeObject(target, settings);
    }

    /// <summary>
    /// Check if an object has default value or not. This extension method works only for value types.
    /// For reference types or nullable value types(which has null value) it always returns false.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool HasDefaultValue(this object value)
    {
        //null is not default value for non-nullable value types
        if (value == null)
            return false;

        Type type = value.GetType();

        // can't be, as would be null
        if (!type.IsValueType)
            return false;

        // ditto, Nullable<T>
        if (Nullable.GetUnderlyingType(type) != null)
            return false;

        object? defaultValue = Activator.CreateInstance(type);
        return value.Equals(defaultValue);
    }
}