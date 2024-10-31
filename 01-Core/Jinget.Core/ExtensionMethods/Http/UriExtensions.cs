namespace Jinget.Core.ExtensionMethods.Http;

public static class UriExtensions
{
    /// <summary>
    /// add querystring to given uri object
    /// </summary>
    /// <param name="uri">base url which the querystring should append to it</param>
    /// <param name="name">querystring key name</param>
    /// <param name="value">querystring value</param>
    public static Uri AddQuery(this Uri uri, string name, string value)
    {
        var option = new UriCreationOptions() { DangerousDisablePathAndQueryCanonicalization = true };
        if (string.IsNullOrWhiteSpace(name))
            return new Uri(uri.AbsoluteUri.TrimEnd('/'), option);
        var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

        httpValueCollection.Remove(name);
        httpValueCollection.Add(name, value);

        var ub = new UriBuilder(uri)
        {
            Query = httpValueCollection.ToString()
        };
        var uriString =
            ub.Scheme +
            Uri.SchemeDelimiter +
            ub.Host + (ub.ToString().Contains(ub.Port.ToString()) ? ":" + uri.Port : "") +
            (ub.Path.Length > 1 ? ub.Path : ub.Path.TrimStart('/')) +
            ub.Query;
        return new Uri(uriString, option);
    }
}
