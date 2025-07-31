namespace Jinget.Core.Utilities.Http;

public class HeaderUtility
{
    /// <summary>
    /// check if content type header presents in the given collection or not.
    /// This method does search for the key using <seealso cref="StringComparison.OrdinalIgnoreCase"/>
    /// </summary>
    public static bool HasContentType(Dictionary<string, string> headers)
    =>
        headers.Keys.Any(x => string.Equals(x, "Content-Type", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// searches for content-type header and if presents, return its value.
    /// This method searches for the content-type header using <seealso cref="HasContentType(Dictionary{string, string})"/>
    /// </summary>
    public static string GetContentTypeValue(Dictionary<string, string> headers)
    {
        if (HasContentType(headers))
        {
            var headerValue = headers.FirstOrDefault(x => string.Equals(x.Key, "Content-Type", StringComparison.OrdinalIgnoreCase)).Value;
            return headerValue.Split(';', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        }
        return "";
    }

    /// <summary>
    /// searches for content-type header and if presents, return its key.
    /// This method searches for the content-type header using <seealso cref="HasContentType(Dictionary{string, string})"/>
    /// </summary>
    public static string GetContentTypeHeaderName(Dictionary<string, string> headers)
    {
        if (HasContentType(headers))
            return headers.FirstOrDefault(x => string.Equals(x.Key, "Content-Type", StringComparison.OrdinalIgnoreCase)).Key;
        return "";
    }

    /// <summary>
    /// check if <seealso cref="MediaTypeNames.Application.Xml"/> 
    /// or <seealso cref="MediaTypeNames.Text.Xml"/> exists in the given header collection
    /// </summary>
    public static bool IsXmlContentType(Dictionary<string, string> headers)
        =>
        HasContentType(headers) &&
        (string.Equals(GetContentTypeValue(headers), MediaTypeNames.Text.Xml, StringComparison.OrdinalIgnoreCase) ||
        GetContentTypeValue(headers).StartsWith(MediaTypeNames.Application.Xml, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// check if <seealso cref="MediaTypeNames.Application.Json"/> exists in the given header collection
    /// </summary>
    public static bool IsJsonContentType(Dictionary<string, string> headers)
        =>
        HasContentType(headers) &&
        GetContentTypeValue(headers).Equals(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// check if <seealso cref="MediaTypeNames.Multipart.FormData"/> exists in the given header collection
    /// </summary>
    public static bool IsMultiPartFormDataContentType(Dictionary<string, string> headers)
        =>
        HasContentType(headers) &&
        GetContentTypeValue(headers).Equals(MediaTypeNames.Multipart.FormData, StringComparison.OrdinalIgnoreCase);
}
