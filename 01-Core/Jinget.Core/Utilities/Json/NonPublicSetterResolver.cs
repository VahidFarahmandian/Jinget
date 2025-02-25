using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jinget.Core.Utilities.Json;

/// <summary>
/// Serialize even properties with non public setter
/// </summary>
public class NonPublicSetterConverter<T> : JsonConverter<T> where T : class
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
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

                PropertyInfo property = typeToConvert.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null && property.GetSetMethod(true) != null) // Check for any setter (public or non-public)
                {
                    try
                    {
                        object value = JsonSerializer.Deserialize(ref reader, property.PropertyType, options);
                        property.SetValue(obj, value);
                    }
                    catch (JsonException)
                    {
                        // Skip the value if deserialization fails
                        JsonSerializer.Deserialize(ref reader, typeof(object), options);
                    }
                }
                else
                {
                    // Skip the value if the property doesn't exist or doesn't have a setter
                    JsonSerializer.Deserialize(ref reader, typeof(object), options);
                }
            }
        }
        throw new JsonException("Unexpected end of JSON data.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetGetMethod() != null)
            {
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, property.GetValue(value), options);
            }
        }

        writer.WriteEndObject();
    }
}