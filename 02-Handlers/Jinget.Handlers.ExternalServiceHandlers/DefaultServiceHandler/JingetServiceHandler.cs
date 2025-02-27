namespace Jinget.Handlers.ExternalServiceHandlers.DefaultServiceHandler;

/// <summary>
/// Service handler for processing HTTP responses with a specific response model.
/// </summary>
/// <typeparam name="TResponseModel">The type of the response model.</typeparam>
public class JingetServiceHandler<TResponseModel> : JingetServiceHandlerBase<JingetServiceHandlerEvents<TResponseModel>> where TResponseModel : class, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JingetServiceHandler{TResponseModel}"/> class with the specified base URI, timeout, and SSL error handling.
    /// </summary>
    /// <param name="baseUrl">The base URI for the service.</param>
    /// <param name="timeout">The timeout for HTTP requests.</param>
    /// <param name="ignoreSslErrors">A value indicating whether to ignore SSL errors.</param>
    public JingetServiceHandler(IServiceProvider serviceProvider, string baseUrl, string clientName = "jinget-client", TimeSpan? timeout = null) : base(serviceProvider, baseUrl, clientName, timeout) { }

    /// <summary>
    /// Processes the HTTP response, deserializes it into the specified response model, and invokes the ResponseDeserializedAsync event if successful.
    /// </summary>
    /// <param name="rawResponse">The raw response content as a string.</param>
    /// <param name="httpResponse">The HTTP response message.</param>
    /// <returns>The deserialized response model, or <c>null</c> if deserialization fails or an error occurs.</returns>
    private async Task<TResponseModel?> ProcessResponseAsync(string rawResponse, HttpResponseMessage httpResponse)
    {
        var deserializedResponse = await base.ProcessResponseDefaultAsync<TResponseModel>(rawResponse, httpResponse);

        if (deserializedResponse != null && !EqualityComparer<TResponseModel>.Default.Equals(deserializedResponse, default))
        {
            await Events.OnResponseDeserializedAsync(deserializedResponse);
        }
        return deserializedResponse;
    }

    /// <summary>
    /// Makes an HTTP GET request to the specified URL and processes the response.
    /// </summary>
    /// <param name="requestUrl">The URL to make the GET request to.</param>
    /// <param name="requestHeaders">Optional headers to include in the request.</param>
    /// <returns>The deserialized response model, or <c>null</c> if an error occurs.</returns>
    public async Task<TResponseModel?> GetAsync(string requestUrl, Dictionary<string, string>? requestHeaders = null)
        => await base.GetAsync(requestUrl, requestHeaders, ProcessResponseAsync);

    /// <summary>
    /// Makes an HTTP POST request with the specified content and processes the response.
    /// </summary>
    /// <param name="requestBody">The content to include in the POST request.</param>
    /// <param name="requestHeaders">Optional headers to include in the request.</param>
    /// <returns>The deserialized response model, or <c>null</c> if an error occurs.</returns>
    public async Task<TResponseModel?> PostAsync(object? requestBody = null, Dictionary<string, string>? requestHeaders = null)
        => await base.PostAsync(requestBody, requestHeaders, ProcessResponseAsync);

    /// <summary>
    /// Makes an HTTP POST request to the specified URL with the specified content and processes the response.
    /// </summary>
    /// <param name="requestUrl">The URL to make the POST request to.</param>
    /// <param name="requestBody">The content to include in the POST request.</param>
    /// <param name="requestHeaders">Optional headers to include in the request.</param>
    /// <returns>The deserialized response model, or <c>null</c> if an error occurs.</returns>
    public async Task<TResponseModel?> PostAsync(string requestUrl, object? requestBody = null, Dictionary<string, string>? requestHeaders = null)
        => await base.PostAsync(requestUrl, requestBody, requestHeaders, ProcessResponseAsync);

    /// <summary>
    /// Uploads files to the specified URL and processes the response.
    /// </summary>
    /// <param name="requestUrl">The URL to upload the files to.</param>
    /// <param name="files">The list of files to upload.</param>
    /// <param name="requestHeaders">Optional headers to include in the request.</param>
    /// <returns>The deserialized response model, or <c>null</c> if an error occurs.</returns>
    public async Task<TResponseModel?> UploadFilesAsync(string requestUrl, List<FileInfo>? files = null, Dictionary<string, string>? requestHeaders = null)
        => await base.UploadFilesAsync(requestUrl, files, requestHeaders, ProcessResponseAsync);

    /// <summary>
    /// Uploads files to the specified URL using a multipart form data content and processes the response.
    /// </summary>
    /// <param name="requestUrl">The URL to upload the files to.</param>
    /// <param name="multipartFormData">The multipart form data content.</param>
    /// <param name="requestHeaders">Optional headers to include in the request.</param>
    /// <returns>The deserialized response model, or <c>null</c> if an error occurs.</returns>
    public async Task<TResponseModel?> UploadFilesAsync(string requestUrl, MultipartFormDataContent? multipartFormData = null, Dictionary<string, string>? requestHeaders = null)
        => await base.UploadFilesAsync(requestUrl, multipartFormData, requestHeaders, ProcessResponseAsync);

    /// <summary>
    /// Sends an HTTP request message and processes the response.
    /// </summary>
    /// <param name="httpRequestMessage">The HTTP request message to send.</param>
    /// <returns>The deserialized response model, or <c>null</c> if an error occurs.</returns>
    public async Task<TResponseModel?> SendAsync(HttpRequestMessage httpRequestMessage)
        => await base.SendAsync(httpRequestMessage, ProcessResponseAsync);
}

/// <summary>
/// Service handler for processing HTTP responses as raw strings.
/// </summary>
public class JingetServiceHandler : JingetServiceHandlerBase<JingetServiceHandlerEvents>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JingetServiceHandler"/> class with the specified base URI, timeout, and SSL error handling.
    /// </summary>
    /// <param name="baseUrl">The base URI for the service.</param>
    /// <param name="timeout">The timeout for HTTP requests.</param>
    /// <param name="ignoreSslErrors">A value indicating whether to ignore SSL errors.</param>
    public JingetServiceHandler(IServiceProvider serviceProvider, string baseUrl, string clientName = "jinget-client", TimeSpan? timeout = null) : base(serviceProvider, baseUrl, clientName, timeout) { }
}