using Newtonsoft.Json.Linq;

namespace Jinget.Core.Utilities.Json;

public class JsonUtility
{
    /// <summary>
    /// check if given string is a valid json string or not.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValid(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) { return false; }
        input = input.Trim();
        if ((input.StartsWith("{") && input.EndsWith("}")) || //For object
            (input.StartsWith("[") && input.EndsWith("]"))) //For array
        {
            try
            {
                var tmpObj = JToken.Parse(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
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
    /// <returns></returns>
    public static string Merge(string json1, string json2, string propertyName, bool unescapeResult = true)
    {
        var token = JToken.Parse(json1);
        if (token is JObject)
        {
            var opJson = JObject.Parse(json1);
            opJson.Add(propertyName, JToken.Parse(json2));
            if (unescapeResult)
                return Unescape(opJson.ToString());
            return opJson.ToString();
        }
        else
            throw new System.Exception($"{nameof(json1)} should be an object");
    }
}