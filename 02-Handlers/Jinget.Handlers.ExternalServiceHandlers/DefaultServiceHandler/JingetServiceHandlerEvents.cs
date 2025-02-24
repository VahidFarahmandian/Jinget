namespace Jinget.Handlers.ExternalServiceHandlers.DefaultServiceHandler;

public class JingetServiceHandlerEvents
{
    public event EventHandler<HttpResponseMessage>? ServiceCalled;
    public event EventHandler<string>? RawResponseReceived;
    public event EventHandler<Exception>? ExceptionOccurred;

    public virtual void OnServiceCalled(HttpResponseMessage e) => ServiceCalled?.Invoke(this, e);
    public virtual void OnRawResponseReceived(string e) => RawResponseReceived?.Invoke(this, e);
    public virtual void OnExceptionOccurred(Exception e) => ExceptionOccurred?.Invoke(this, e);
}

public class JingetServiceHandlerEvents<TResponseModel> : JingetServiceHandlerEvents where TResponseModel : class, new()
{
    public event EventHandler<TResponseModel?>? ResponseDeserialized;

    public virtual void OnResponseDeserialized(TResponseModel? e) => ResponseDeserialized?.Invoke(this, e);
}
