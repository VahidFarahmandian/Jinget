namespace Jinget.Core.Utilities;

public abstract class SOAPRequestBase<TEnvelope, TRequest, TViewModel>
{
    public SOAPRequestBase() { }

    public abstract (TEnvelope envelope, TRequest request) CreateEnvelope(TViewModel vm);
}

public abstract class SOAPRequestBase<TEnvelope, TRequest>
{
    public SOAPRequestBase() { }

    public abstract (TEnvelope? envelope, TRequest? request) CreateEnvelope();
}

public abstract class SOAPEnvelopeBase
{
    public virtual XmlSerializerNamespaces GetRequestNamespaces()
    {
        XmlSerializerNamespaces ns = new();
        ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance/");
        ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
        ns.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");

        return ns;
    }
}
