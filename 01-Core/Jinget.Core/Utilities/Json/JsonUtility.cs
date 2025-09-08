using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Jinget.Core.Utilities.Json;

public class JsonUtility
{
    /// <summary>
    /// check if given string is a valid json string or not.
    /// </summary>
    public static bool IsValid(string? jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return false;
        }

        try
        {
            JsonDocument.Parse(jsonString);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// unescape json string and replace the illegal strings
    /// </summary>
    public static string Unescape(string json, bool removeNewLine = true)
    {
        json = json
         .Replace(@"\\\""", "\"") // ==? \\"" ==> " 
         .Replace(@"\""", "\"") //==> "" ==> "
         .Replace("\"[{\"", "[{\"")//==> "[{" ==> [{"
         .Replace("\"}]\"", "\"}]") //==> "}]" ==> "}]
         .Replace("\"{", "{")// =="{ ==> {
         .Replace("}\"", "}")//==> }" ==> }
         .Replace("\"[", "[")//==> "[ ==> [
         .Replace("]\"", "]") //==> ]" ==> ]

         //order of the follwoings are matter!
         .Replace(":\"\",", ":\"---\",")//==> :"", ==> :"---",
         .Replace(":\"\"}", ":\"---\"}")//==> :""} ==> :"---"}
         .Replace(":\"\"", ":\"")//==> :"" ==> :"
         .Replace("\"\",", "\",");//==> "" ==> "

        if (removeNewLine)
        {
            json = json
                .Replace(Environment.NewLine, "")
                .Replace(@"\r\n", "");
        }

        return json;
    }

    /// <summary>
    /// merge <paramref name="json1"/> with <paramref name="json2"/> using <paramref name="propertyName"/> value
    /// </summary>
    public static string Merge(string json1, string json2, string propertyName, bool unescapeResult = true)
    {
        try
        {
            var jsonNode1 = JsonNode.Parse(json1);

            if (jsonNode1 is JsonObject jsonObject1)
            {
                var jsonNode2 = JsonNode.Parse(json2);
                jsonObject1[propertyName] = jsonNode2;

                string result = jsonObject1.ToJsonString(new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

                if (unescapeResult)
                {
                    return Unescape(result);
                }

                return result;
            }
            else
            {
                throw new Exception($"{nameof(json1)} should be an object");
            }
        }
        catch (JsonException ex)
        {
            throw new Exception($"Invalid JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred: {ex.Message}");
        }
    }
}