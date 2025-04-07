namespace Jinget.Core.ExtensionMethods.HttpContext;

/// <summary>
/// Provides extension methods for <see cref="HttpRequest"/> operations.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Determines whether the current HTTP request is a CORS preflight request.
    /// </summary>
    /// <param name="request">The HTTP request to check.</param>
    /// <returns>
    /// <c>true</c> if the request is an OPTIONS request with both Origin and 
    /// Access-Control-Request-Method headers present; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// A CORS preflight request is an OPTIONS request that browsers send before making 
    /// certain cross-origin requests to check if the server allows the actual request.
    /// </remarks>
    public static bool IsPreflight(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HttpMethods.IsOptions(request.Method) &&
               request.Headers.TryGetValue("Origin", out _) &&
               request.Headers.TryGetValue("Access-Control-Request-Method", out _);
    }
}