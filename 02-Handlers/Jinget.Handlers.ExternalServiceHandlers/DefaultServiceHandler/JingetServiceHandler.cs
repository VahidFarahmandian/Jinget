namespace Jinget.Handlers.ExternalServiceHandlers.DefaultServiceHandler;

public class JingetServiceHandler<TResponseModel> : ServiceHandler<JingetServiceHandlerEvents<TResponseModel>> where TResponseModel : class, new()
{
    public JingetServiceHandler(string baseUri, bool ignoreSslErrors = false) : base(baseUri, ignoreSslErrors) { }
    public JingetServiceHandler(string baseUri, TimeSpan timeout, bool ignoreSslErrors = false) : base(baseUri, timeout, ignoreSslErrors) { }

    private async Task<TResponseModel?> ProcessTaskAsync(Func<Task<HttpResponseMessage>> task)
    {
        TResponseModel? responseModel = default;

        try
        {
            var response = await task();
            Events?.OnServiceCalled(response);

            response.EnsureSuccessStatusCode();

            string rawResponse = await response.Content.ReadAsStringAsync();
            Events?.OnRawResponseReceived(rawResponse);

            if (response.Content.Headers.ContentType != null)
            {
                switch (response.Content.Headers.ContentType.MediaType)
                {
                    case MediaTypeNames.Application.Json:
                        responseModel = JsonConvert.DeserializeObject<TResponseModel>(rawResponse);
                        break;
                    case MediaTypeNames.Application.Xml:
                    case MediaTypeNames.Text.Xml:
                        responseModel = DeserializeXmlDescendantsFirst<TResponseModel>(rawResponse);
                        break;
                    default:
                        // Handle unknown media type.
                        Events?.OnExceptionOccurred(new InvalidOperationException($"Unsupported media type: {response.Content.Headers.ContentType.MediaType}"));
                        break;
                }
            }
            else
            {
                Events?.OnExceptionOccurred(new InvalidOperationException("content type was null"));
            }

            Events?.OnResponseDeserialized(responseModel); // Null check
        }
        catch (Exception ex)
        {
            Events?.OnExceptionOccurred(ex); // Null check
            return default; // or throw the exception.
        }

        return responseModel;
    }

    public async Task<TResponseModel?> GetAsync(string url, Dictionary<string, string>? headers = null)
        => await ProcessTaskAsync(async () => await HttpClientFactory.GetAsync(url, headers));

    public async Task<TResponseModel?> PostAsync(object? content = null, Dictionary<string, string>? headers = null)
        => await ProcessTaskAsync(async () => await HttpClientFactory.PostAsync("", content, headers));

    public async Task<TResponseModel?> PostAsync(string url, object? content = null, Dictionary<string, string>? headers = null)
        => await ProcessTaskAsync(async () => await HttpClientFactory.PostAsync(url, content, headers));

    public async Task<TResponseModel?> UploadFileAsync(string url, List<FileInfo>? files = null, Dictionary<string, string>? headers = null)
        => await ProcessTaskAsync(async () => await HttpClientFactory.UploadFileAsync(url, files, headers));

    public async Task<TResponseModel?> UploadFileAsync(string url, MultipartFormDataContent? multipartFormData = null, Dictionary<string, string>? headers = null)
        => await ProcessTaskAsync(async () => await HttpClientFactory.UploadFileAsync(url, multipartFormData, headers));

    public async Task<TResponseModel?> SendAsync(HttpRequestMessage message) => await ProcessTaskAsync(async () => await HttpClientFactory.SendAsync(message));
}
