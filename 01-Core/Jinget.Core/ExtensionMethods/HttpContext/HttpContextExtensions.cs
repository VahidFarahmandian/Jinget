using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Net;

namespace Jinget.Core.ExtensionMethods.HttpContext;

public static class HttpContextExtensions
{
    /// <summary>
    /// Determines whether the current HTTP request has a multipart content type (specifically "multipart/form-data").
    /// </summary>
    /// <param name="context">The HttpContext representing the current HTTP request.</param>
    /// <returns>True if the content type is multipart/form-data; otherwise, false.</returns>
    /// <remarks>
    /// This method checks the Content-Type header of the request to determine if it starts with "multipart/form-data".
    /// It uses the GetTypedHeaders() extension method for robust header parsing and performs a case-insensitive comparison.
    /// </remarks>
    public static bool IsMultipartContentType(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        // Retrieve the parsed Content-Type header from the request.
        var contentTypeHeader = context.Request.GetTypedHeaders().ContentType;

        // Check if the Content-Type header exists.
        if (contentTypeHeader != null)
        {
            // Retrieve the media type value from the parsed header.
            var mediaType = contentTypeHeader.MediaType.Value;

            // Check if the media type value exists and starts with "multipart/form-data" (case-insensitive).
            if (mediaType != null && mediaType.StartsWith("multipart/form-data", StringComparison.CurrentCultureIgnoreCase))
            {
                return true; // It's a multipart/form-data request.
            }
        }

        return false; // It's not a multipart/form-data request.
    }

    /// <summary>
    /// Retrieves the client's IP address from the HttpContext.
    /// </summary>
    /// <param name="context">The HttpContext from which to retrieve the IP address.</param>
    /// <param name="customClientIpHeader">
    ///     The header name containing the client's IP address (e.g., "X-Forwarded-For").
    ///     Defaults to "X-Forwarded-For".
    /// </param>
    /// <returns>
    ///     The client's IP address as a string, or "Unknown" if the IP address cannot be determined.
    /// </returns>
    /// <remarks>
    ///     This method first attempts to retrieve the IP address from the specified custom header.
    ///     If the header is not found or the IP address in the header is invalid, it falls back to the
    ///     RemoteIpAddress property of the HttpContext.Connection.
    ///     The RemoteIpAddress property represents the IP address of the immediate connection to the server.
    ///     In environments with reverse proxies or load balancers, the RemoteIpAddress may not represent
    ///     the actual client's IP address.
    ///     The method also validates the IP address retrieved from the custom header using IPAddress.TryParse.
    /// </remarks>
    public static string GetClientIpAddress(this Microsoft.AspNetCore.Http.HttpContext context, string customClientIpHeader = "X-Forwarded-For")
    {
        // Get the IP address of the immediate connection to the server.
        // This may be the proxy or load balancer's IP, not the client's.
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        // Check if the specified custom header contains the client's IP address.
        if (context.Request.Headers.TryGetValue(customClientIpHeader, out var clientIpAddress))
        {
            // Get the first IP address from the header (in case of multiple IPs).
            var firstIp = clientIpAddress.ToString().Split(',')[0].Trim();

            // Validate the IP address format.
            if (IPAddress.TryParse(firstIp, out _))
            {
                // If valid, use the IP address from the header.
                ipAddress = firstIp;
            }
        }

        // Return the retrieved IP address, or "Unknown" if not found.
        return ipAddress ?? "Unknown";
    }

    /// <summary>
    /// Checks if the endpoint associated with the current HTTP request has the [Authorize] attribute.
    /// </summary>
    /// <param name="httpContext">The HttpContext for the current request.</param>
    /// <returns>True if the endpoint has the [Authorize] attribute; otherwise, false.</returns>
    /// <remarks>
    /// This method retrieves the endpoint from the HttpContext and checks for the presence of the AuthorizeAttribute.
    /// It returns true if the attribute is found; otherwise, it returns false.
    /// This method does not evaluate authorization policies. It only checks for the existence of the attribute.
    /// </remarks>
    public static bool EndpointIsDecoratedWithAuthorizeAttribute(this Microsoft.AspNetCore.Http.HttpContext httpContext)
        => httpContext.GetEndpoint()?.Metadata.GetMetadata<AuthorizeAttribute>() != null;

    /// <summary>
    /// Retrieves all unique claim titles from API endpoint metadata decorated with the specified claim attribute type
    /// </summary>
    /// <typeparam name="TClaimAttribute">The type of claim attribute to search for (must inherit from <seealso cref="ClaimAttribute"/>)</typeparam>
    /// <param name="httpContext">The current HTTP context</param>
    /// <returns>Distinct collection of claim titles found in API endpoints</returns>
    public static IEnumerable<string> GetClaimTitlesFromEndpoints<TClaimAttribute>(this Microsoft.AspNetCore.Http.HttpContext httpContext)
        where TClaimAttribute : ClaimAttribute
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var apiDescriptionProvider = httpContext.RequestServices.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

        return apiDescriptionProvider.ApiDescriptionGroups.Items
            .SelectMany(group => group.Items)
            .SelectMany(description => description.ActionDescriptor.EndpointMetadata)
            .OfType<TClaimAttribute>()
            .Select(attribute => attribute.Title)
            .Distinct();
    }
}