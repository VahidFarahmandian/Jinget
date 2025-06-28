using Jinget.Core.Utilities.Json;
using System.Text.Json;

namespace Jinget.Core.ExtensionMethods;

public static class JsonExtensions
{
    /// <summary>
    /// Deserializes a JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to. Must be non-nullable and have a parameterless constructor.</typeparam>
    /// <param name="serializedString">The JSON string to deserialize. Can be null or whitespace.</param>
    /// <param name="strictPropertyMatching">Properties in JSON string must exactly match the properties name in type. comparison is case sensitive</param>
    /// <returns>The deserialized object, or a new instance of T if the input is null or whitespace or deserialization fails.</returns>
    public static T? Deserialize<T>(this string? serializedString, JsonSerializerOptions? options = null, bool strictPropertyMatching = false) where T : class
    {
        if (JsonUtility.IsValid(serializedString) == false)
        {
            throw new JsonException($"input string is not valid json string'.");
        }

        var defaultOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        options = options ?? defaultOptions;

        T? deserializedValue = default;
        try
        {
            deserializedValue = JsonSerializer.Deserialize<T>(serializedString, options);
        }
        catch (JsonException ex)
        {
            // Attempt to parse the JSON to inspect it
            using JsonDocument doc = JsonDocument.Parse(serializedString);
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                bool needsReplacement = false;
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.True ||
                        property.Value.ValueKind == JsonValueKind.False ||
                        property.Value.ValueKind == JsonValueKind.Number)
                    {
                        needsReplacement = true;
                        break;
                    }
                }

                if (needsReplacement)
                {
                    string pattern = @":(true|false|\d+)";

                    string replacedJson = Regex.Replace(serializedString, pattern, match =>
                    {
                        return $":\"{match.Groups[1].Value.ToLower()}\"";
                    }, RegexOptions.IgnoreCase);


                    deserializedValue = JsonSerializer.Deserialize<T>(replacedJson, options);

                }
            }
        }

        if (strictPropertyMatching && deserializedValue != null)
        {
            var jsonDocument = JsonDocument.Parse(serializedString);
            var root = jsonDocument.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                var typeProperties = typeof(T).GetProperties().Select(p => p.Name).ToHashSet();

                foreach (var property in root.EnumerateObject())
                {
                    if (!typeProperties.Contains(property.Name))
                    {
                        throw new JsonException($"Property '{property.Name}' not found in type '{typeof(T).Name}'.");
                    }
                }
            }
        }

        return deserializedValue;
    }

    /// <summary>
    /// Serializes an object of type T to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize. Must be non-nullable and have a parameterless constructor.</typeparam>
    /// <param name="typeValue">The object to serialize. Can be null.</param>
    /// <returns>The JSON string representation of the object, or an empty string if the input is null or serialization fails.</returns>
    public static string Serialize<T>(this T? typeValue, JsonSerializerOptions? options = null) where T : class
    {
        // Check if the input object is null.
        if (typeValue == null)
        {
            // If so, return an empty string.
            return "";
        }

        try
        {
            // Attempt to serialize the object.
            var serializedValue = System.Text.Json.JsonSerializer.Serialize<T>(typeValue, options);

            // If serialization was successful and the result is not null or whitespace, return the serialized string.
            // Otherwise, return an empty string.
            return string.IsNullOrWhiteSpace(serializedValue) ? "" : serializedValue;
        }
        catch (System.Text.Json.JsonException)
        {
            // Handle JSON serialization exceptions by returning an empty string.
            // Consider logging the exception for debugging purposes.
            return "";
        }
    }
}