using System.Linq;

namespace Jinget.Handlers.ExternalServiceHandlers.DefaultServiceHandler;

/// <summary>
/// Represents events related to Jinget service handling.
/// </summary>
public class JingetServiceHandlerEvents
{
    /// <summary>
    /// Occurs when a service call is made.
    /// </summary>
    public event Func<object, HttpResponseMessage, Task>? ServiceCalledAsync;

    /// <summary>
    /// Occurs when a raw response is received from the service.
    /// </summary>
    public event Func<object, string, Task>? RawResponseReceivedAsync;

    /// <summary>
    /// Occurs when an exception occurs during service handling.
    /// </summary>
    public event Func<object, Exception, Task>? ExceptionOccurredAsync;

    /// <summary>
    /// Invokes the <see cref="ServiceCalledAsync"/> event handlers.
    /// </summary>
    /// <param name="httpResponseMessage">The HTTP response message.</param>
    public async Task OnServiceCalledAsync(HttpResponseMessage httpResponseMessage)
    {
        if (ServiceCalledAsync != null)
        {
            foreach (var handler in ServiceCalledAsync.GetInvocationList().Cast<Func<object, HttpResponseMessage, Task>>())
            {
                await handler(this, httpResponseMessage);
            }
        }
    }

    /// <summary>
    /// Invokes the <see cref="RawResponseReceivedAsync"/> event handlers.
    /// </summary>
    /// <param name="rawResponse">The raw response content as a string.</param>
    public async Task OnRawResponseReceivedAsync(string rawResponse)
    {
        if (RawResponseReceivedAsync != null)
        {
            foreach (var handler in RawResponseReceivedAsync.GetInvocationList().Cast<Func<object, string, Task>>())
            {
                await handler(this, rawResponse);
            }
        }
    }

    /// <summary>
    /// Invokes the <see cref="ExceptionOccurredAsync"/> event handlers.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    public async Task OnExceptionOccurredAsync(Exception exception)
    {
        if (ExceptionOccurredAsync != null)
        {
            foreach (var handler in ExceptionOccurredAsync.GetInvocationList().Cast<Func<object, Exception, Task>>())
            {
                await handler(this, exception);
            }
        }
    }
}

/// <summary>
/// Represents events related to Jinget service handling with a specific response model.
/// </summary>
/// <typeparam name="TResponseModel">The type of the response model.</typeparam>
public class JingetServiceHandlerEvents<TResponseModel> : JingetServiceHandlerEvents where TResponseModel : class, new()
{
    /// <summary>
    /// Occurs when a response is deserialized into the specified response model.
    /// </summary>
    public event Func<object, TResponseModel?, Task>? ResponseDeserializedAsync;

    /// <summary>
    /// Invokes the <see cref="ResponseDeserializedAsync"/> event handlers.
    /// </summary>
    /// <param name="responseModel">The deserialized response model.</param>
    public async Task OnResponseDeserializedAsync(TResponseModel? responseModel)
    {
        if (ResponseDeserializedAsync != null)
        {
            foreach (var handler in ResponseDeserializedAsync.GetInvocationList().Cast<Func<object, TResponseModel?, Task>>())
            {
                await handler(this, responseModel);
            }
        }
    }
}