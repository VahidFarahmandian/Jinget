namespace Jinget.Core.ExtensionMethods;

public static class HttpContextExtensions
{
    /// <summary>
    /// Check if given request is a multipart request or not
    /// </summary>
    public static bool IsMultipartContentType(this HttpContext context) =>
        context.Request.GetTypedHeaders().ContentType != null &&
        context.Request.GetTypedHeaders().ContentType.MediaType.Value.ToLower().StartsWith("multipart/form-data");

    /// <summary>
    /// Get request connecton ip address
    /// </summary>
    public static string GetIpAddress(this HttpContext context) =>
        context.Connection.RemoteIpAddress == null
            ? "Unknown"
            : context.Connection.RemoteIpAddress.ToString();

    public static bool EndpointIsAuthorized(this HttpContext httpContext)
        => httpContext.GetEndpoint()?.Metadata.GetMetadata<AuthorizeAttribute>() != null;
}