namespace Jinget.Core.Utilities;

/// <summary>
/// Provides a base class for creating SOAP request envelopes with a ViewModel.
/// </summary>
/// <typeparam name="TEnvelope">The type of the SOAP envelope.</typeparam>
/// <typeparam name="TRequest">The type of the SOAP request body.</typeparam>
/// <typeparam name="TViewModel">The type of the ViewModel used to create the envelope.</typeparam>
public abstract class SOAPRequestBase<TEnvelope, TRequest, TViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SOAPRequestBase{TEnvelope, TRequest, TViewModel}"/> class.
    /// </summary>
    protected SOAPRequestBase() { }

    /// <summary>
    /// Creates a SOAP envelope and request using the provided ViewModel.
    /// </summary>
    /// <param name="vm">The ViewModel used to populate the SOAP request.</param>
    /// <returns>A tuple containing the SOAP envelope and request.</returns>
    public abstract (TEnvelope envelope, TRequest request) CreateEnvelope(TViewModel vm);
}

/// <summary>
/// Provides a base class for creating SOAP request envelopes without a ViewModel.
/// </summary>
/// <typeparam name="TEnvelope">The type of the SOAP envelope.</typeparam>
/// <typeparam name="TRequest">The type of the SOAP request body.</typeparam>
public abstract class SOAPRequestBase<TEnvelope, TRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SOAPRequestBase{TEnvelope, TRequest}"/> class.
    /// </summary>
    protected SOAPRequestBase() { }

    /// <summary>
    /// Creates a SOAP envelope and request.
    /// </summary>
    /// <returns>A tuple containing the SOAP envelope and request, or null if creation fails.</returns>
    public abstract (TEnvelope? envelope, TRequest? request) CreateEnvelope();
}

/// <summary>
/// Provides a base class for SOAP envelope creation and namespace configuration.
/// </summary>
public abstract class SOAPEnvelopeBase
{
    /// <summary>
    /// Gets the XML namespaces used in the SOAP request.
    /// </summary>
    /// <returns>The XML namespaces for the SOAP request.</returns>
    public virtual XmlSerializerNamespaces GetRequestNamespaces()
    {
        XmlSerializerNamespaces ns = new();
        ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance/");
        ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
        ns.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");

        return ns;
    }
}