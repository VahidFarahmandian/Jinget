using Newtonsoft.Json.Linq;

namespace Jinget.Core.Utilities.Json
{
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
    }
}