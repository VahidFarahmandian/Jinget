using System.Text.Json;

namespace Jinget.Core.Utilities.Json;

/// <summary>
/// ignore the given properties, while serializing an object
/// </summary>
public class IgnorePropertiesResolver<T> : System.Text.Json.Serialization.JsonConverter<T> where T : class
{
    private readonly string[] _propertiesToIgnore;

    public IgnorePropertiesResolver(string[] propertiesToIgnore)
    {
        _propertiesToIgnore = propertiesToIgnore ?? Array.Empty<string>();
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new System.Text.Json.JsonException();
        }

        var obj = Activator.CreateInstance<T>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return obj;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read(); // Move to property value

                if (!_propertiesToIgnore.Contains(propertyName))
                {
                    PropertyInfo property = typeToConvert.GetProperty(propertyName);
                    if (property != null && property.CanWrite)
                    {
                        try
                        {
                            object value = System.Text.Json.JsonSerializer.Deserialize(ref reader, property.PropertyType, options);
                            property.SetValue(obj, value);
                        }
                        catch (System.Text.Json.JsonException)
                        {
                            // Skip the value if deserialization fails
                            System.Text.Json.JsonSerializer.Deserialize(ref reader, typeof(object), options);
                        }

                    }
                    else
                    {
                        // Skip the value if the property doesn't exist or can't be written
                        System.Text.Json.JsonSerializer.Deserialize(ref reader, typeof(object), options);
                    }
                }
                else
                {
                    // Skip the value if the property is ignored
                    System.Text.Json.JsonSerializer.Deserialize(ref reader, typeof(object), options);
                }
            }
        }
        throw new System.Text.Json.JsonException("Unexpected end of JSON data.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var property in typeof(T).GetProperties())
        {
            if (!_propertiesToIgnore.Contains(property.Name))
            {
                writer.WritePropertyName(property.Name);
                System.Text.Json.JsonSerializer.Serialize(writer, property.GetValue(value), options);
            }
        }

        writer.WriteEndObject();
    }
}
