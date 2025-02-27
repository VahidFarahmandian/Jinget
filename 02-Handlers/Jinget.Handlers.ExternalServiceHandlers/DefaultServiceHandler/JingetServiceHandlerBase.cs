using Jinget.Core.ExtensionMethods;

namespace Jinget.Handlers.ExternalServiceHandlers.DefaultServiceHandler;

/// <summary>
/// Abstract base class for Jinget service handlers. Provides common functionality for making HTTP requests and processing responses.
/// </summary>
/// <typeparam name="TEvents">The type of events associated with this service handler.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="JingetServiceHandlerBase{TEvents}"/> class with the specified base URI, timeout, and SSL error handling.
/// </remarks>
/// <param name="baseUri">The base URI for the service.</param>
/// <param name="timeout">The timeout for HTTP requests.</param>
/// <param name="ignoreSslErrors">A value indicating whether to ignore SSL errors.</param>
public abstract class JingetServiceHandlerBase<TEvents>(IServiceProvider serviceProvider, string baseUri, string clientName = "jinget-client", TimeSpan? timeout = null) : ServiceHandler<TEvents>(serviceProvider, baseUri, clientName, timeout) where TEvents : JingetServiceHandlerEvents, new()
{

    /// <summary>
    /// Processes an asynchronous HTTP request task, handles the response, and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be returned.</typeparam>
    /// <param name="task">An asynchronous function that returns an <see cref="HttpResponseMessage"/> representing the HTTP response.</param>
    /// <param name="processResponse">An asynchronous function that processes the raw response content and the <see cref="HttpResponseMessage"/>, returning the deserialized result.</param>
    /// <returns>The deserialized result of type <typeparamref name="TResult"/>, or <c>null</c> if an exception occurs or the processing fails.</returns>
    /// <remarks>
    /// This method encapsulates the common steps of making an HTTP request, handling the response, and deserializing the result.
    /// It ensures the response is successful, reads the raw response content, and invokes event handlers for service calls and raw response reception.
    /// If an exception occurs during the process, it invokes the exception event handler and returns the default value for <typeparamref name="TResult"/>.
    /// </remarks>
    protected async Task<TResult?> ProcessTaskAsync<TResult>(
        Func<Task<HttpResponseMessage>> task,
        Func<string, HttpResponseMessage, Task<TResult?>> processResponse) where TResult : class
    {
        try
        {
            // Execute the provided asynchronous task to get the HTTP response.
            var response = await task();

            // Notify listeners that a service call was made, passing the HTTP response.
            await Events.OnServiceCalledAsync(response);

            // Ensure the HTTP response indicates success by throwing an exception for non-success status codes.
            response.EnsureSuccessStatusCode();

            // Read the raw response content as a string.
            string rawResponse = await response.Content.ReadAsStringAsync();

            // Notify listeners that the raw response was received.
            await Events.OnRawResponseReceivedAsync(rawResponse);

            // Process the raw response and the HTTP response using the provided asynchronous function.
            return await processResponse(rawResponse, response);
        }
        catch (Exception ex)
        {
            // Notify listeners that an exception occurred during the process.
            await Events.OnExceptionOccurredAsync(ex);
            throw;
        }
    }
    /// <summary>
    /// Processes the raw HTTP response content and the <see cref="HttpResponseMessage"/> to deserialize the response into the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the response into.</typeparam>
    /// <param name="rawResponse">The raw response content as a string.</param>
    /// <param name="httpResponse">The <see cref="HttpResponseMessage"/> containing the HTTP response details.</param>
    /// <returns>The deserialized result of type <typeparamref name="TResult"/>, or <c>null</c> if deserialization fails or an error occurs.</returns>
    protected async Task<TResult?> ProcessResponseDefaultAsync<TResult>(string rawResponse, HttpResponseMessage httpResponse) where TResult : class
    {
        // Check if the response content has a content type header.
        if (httpResponse.Content.Headers.ContentType != null)
        {
            // Switch based on the media type of the response content.
            switch (httpResponse.Content.Headers.ContentType.MediaType)
            {
                // Handle JSON content.
                case MediaTypeNames.Application.Json:
                    // Deserialize the JSON response into the specified type.
                    return rawResponse.Deserialize<TResult>(strictPropertyMatching: false);

                // Handle XML content.
                case MediaTypeNames.Application.Xml:
                case MediaTypeNames.Text.Xml:
                    // Deserialize the XML response into the specified type.
                    return DeserializeXmlDescendantsFirst<TResult>(rawResponse);
                // Handle unsupported media types.
                default:
                    // If an exception event handler is registered, invoke it with an exception indicating an unsupported media type.
                    if (Events.OnExceptionOccurredAsync != null)
                    {
                        await Events.OnExceptionOccurredAsync(new InvalidOperationException($"Unsupported media type: {httpResponse.Content.Headers.ContentType.MediaType}"));
                    }
                    // Return the default value for the result type to indicate failure.
                    return default;
            }
        }
        // Handle cases where the content type header is null.
        else
        {
            // If an exception event handler is registered, invoke it with an exception indicating a null content type.
            if (Events.OnExceptionOccurredAsync != null)
            {
                await Events.OnExceptionOccurredAsync(new InvalidOperationException("content type was null"));
            }
            // Return the default value for the result type to indicate failure.
            return default;
        }
    }

    /// <summary>
    /// Makes an asynchronous HTTP GET request to the specified URL and processes the response.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be returned.</typeparam>
    /// <param name="url">The URL to make the GET request to.</param>
    /// <param name="headers">Optional headers to include in the request. If not provided, the request will be sent without additional headers.</param>
    /// <param name="processResponse">Optional function to process the raw HTTP response content and the <see cref="HttpResponseMessage"/>. If not provided, the default response processing logic will be used, which handles JSON and XML deserialization.</param>
    /// <returns>The deserialized result, or <c>null</c> if an error occurs or the response cannot be deserialized.</returns>
    /// <remarks>Calling this method directly will not raise <see cref="JingetServiceHandlerEvents{TResponseModel}.ResponseDeserializedAsync"/> event</remarks>
    public virtual async Task<TResult?> GetAsync<TResult>(string url, Dictionary<string, string>? headers = null, Func<string, HttpResponseMessage, Task<TResult?>>? processResponse = null) where TResult : class
        => await ProcessTaskAsync(async () =>
        {
            return await HttpClientFactory.GetAsync(url, headers);
        }, processResponse ?? ((rawResponse, response) => ProcessResponseDefaultAsync<TResult>(rawResponse, response)));

    /// <summary>
    /// Makes an asynchronous HTTP POST request with the specified content and processes the response.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be returned.</typeparam>
    /// <param name="content">The content to include in the POST request. Can be null.</param>
    /// <param name="headers">Optional headers to include in the request. If not provided, the request will be sent without additional headers.</param>
    /// <param name="processResponse">Optional function to process the raw HTTP response content and the <see cref="HttpResponseMessage"/>. If not provided, the default response processing logic will be used, which handles JSON and XML deserialization.</param>
    /// <returns>The deserialized result, or <c>null</c> if an error occurs or the response cannot be deserialized.</returns>
    /// <remarks>Calling this method directly will not raise <see cref="JingetServiceHandlerEvents{TResponseModel}.ResponseDeserializedAsync"/> event</remarks>
    public virtual async Task<TResult?> PostAsync<TResult>(object? content = null, Dictionary<string, string>? headers = null, Func<string, HttpResponseMessage, Task<TResult?>>? processResponse = null) where TResult : class
        => await ProcessTaskAsync(async () =>
        {
            return await HttpClientFactory.PostAsync("", content, headers);
        }, processResponse ?? ((rawResponse, response) => ProcessResponseDefaultAsync<TResult>(rawResponse, response)));

    /// <summary>
    /// Makes an asynchronous HTTP POST request to the specified URL with the specified content and processes the response.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be returned.</typeparam>
    /// <param name="url">The URL to make the POST request to.</param>
    /// <param name="content">The content to include in the POST request. Can be null.</param>
    /// <param name="headers">Optional headers to include in the request. If not provided, the request will be sent without additional headers.</param>
    /// <param name="processResponse">Optional function to process the raw HTTP response content and the <see cref="HttpResponseMessage"/>. If not provided, the default response processing logic will be used, which handles JSON and XML deserialization.</param>
    /// <returns>The deserialized result, or <c>null</c> if an error occurs or the response cannot be deserialized.</returns>
    /// <remarks>Calling this method directly will not raise <see cref="JingetServiceHandlerEvents{TResponseModel}.ResponseDeserializedAsync"/> event</remarks>
    public virtual async Task<TResult?> PostAsync<TResult>(string url, object? content = null, Dictionary<string, string>? headers = null, Func<string, HttpResponseMessage, Task<TResult?>>? processResponse = null) where TResult : class
        => await ProcessTaskAsync(async () =>
        {
            return await HttpClientFactory.PostAsync(url, content, headers);
        }, processResponse ?? ((rawResponse, response) => ProcessResponseDefaultAsync<TResult>(rawResponse, response)));

    /// <summary>
    /// Uploads files to the specified URL and processes the response.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be returned.</typeparam>
    /// <param name="url">The URL to upload the files to.</param>
    /// <param name="files">The list of files to upload. Can be null.</param>
    /// <param name="headers">Optional headers to include in the request. If not provided, the request will be sent without additional headers.</param>
    /// <param name="processResponse">Optional function to process the raw HTTP response content and the <see cref="HttpResponseMessage"/>. If not provided, the default response processing logic will be used, which handles JSON and XML deserialization.</param>
    /// <returns>The deserialized result, or <c>null</c> if an error occurs or the response cannot be deserialized.</returns>
    /// <remarks>Calling this method directly will not raise <see cref="JingetServiceHandlerEvents{TResponseModel}.ResponseDeserializedAsync"/> event</remarks>
    public virtual async Task<TResult?> UploadFilesAsync<TResult>(string url, List<FileInfo>? files = null, Dictionary<string, string>? headers = null, Func<string, HttpResponseMessage, Task<TResult?>>? processResponse = null) where TResult : class
        => await ProcessTaskAsync(async () =>
        {
            return await HttpClientFactory.UploadFilesAsync(url, files, headers);
        }, processResponse ?? ((rawResponse, response) => ProcessResponseDefaultAsync<TResult>(rawResponse, response)));

    /// <summary>
    /// Uploads files to the specified URL using a multipart form data content and processes the response.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be returned.</typeparam>
    /// <param name="url">The URL to upload the files to.</param>
    /// <param name="multipartFormData">The multipart form data content containing the files. Can be null.</param>
    /// <param name="headers">Optional headers to include in the request. If not provided, the request will be sent without additional headers.</param>
    /// <param name="processResponse">Optional function to process the raw HTTP response content and the <see cref="HttpResponseMessage"/>. If not provided, the default response processing logic will be used, which handles JSON and XML deserialization.</param>
    /// <returns>The deserialized result, or <c>null</c> if an error occurs or the response cannot be deserialized.</returns>
    /// <remarks>Calling this method directly will not raise <see cref="JingetServiceHandlerEvents{TResponseModel}.ResponseDeserializedAsync"/> event</remarks>
    public virtual async Task<TResult?> UploadFilesAsync<TResult>(string url, MultipartFormDataContent? multipartFormData = null, Dictionary<string, string>? headers = null, Func<string, HttpResponseMessage, Task<TResult?>>? processResponse = null) where TResult : class
        => await ProcessTaskAsync(async () =>
        {
            return await HttpClientFactory.UploadFilesAsync(url, multipartFormData, headers);
        }, processResponse ?? ((rawResponse, response) => ProcessResponseDefaultAsync<TResult>(rawResponse, response)));

    /// <summary>
    /// Sends an asynchronous HTTP request message and processes the response.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to be returned.</typeparam>
    /// <param name="message">The HTTP request message to send.</param>
    /// <param name="processResponse">Optional function to process the raw HTTP response content and the <see cref="HttpResponseMessage"/>. If not provided, the default response processing logic will be used, which handles JSON and XML deserialization.</param>
    /// <returns>The deserialized result, or <c>null</c> if an error occurs or the response cannot be deserialized.</returns>
    /// <remarks>Calling this method directly will not raise <see cref="JingetServiceHandlerEvents{TResponseModel}.ResponseDeserializedAsync"/> event</remarks>
    public virtual async Task<TResult?> SendAsync<TResult>(HttpRequestMessage message, Func<string, HttpResponseMessage, Task<TResult?>>? processResponse = null) where TResult : class
        => await ProcessTaskAsync(async () =>
        {
            return await HttpClientFactory.SendAsync(message);
        }, processResponse ?? ((rawResponse, response) => ProcessResponseDefaultAsync<TResult>(rawResponse, response)));
}
