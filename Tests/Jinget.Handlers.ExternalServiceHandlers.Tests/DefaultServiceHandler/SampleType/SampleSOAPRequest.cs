using Jinget.Core.Utilities;
using System.Xml.Serialization;

namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler.SampleType;

/// <summary>
/// defines a full soap request for method Add inside the <see cref="http://www.dneonline.com/calculator.asmx"/>
/// </summary>
public class SampleSOAPRequest : SOAPRequestBase<SampleSOAPRequest.Envelope, SampleSOAPRequest.SampleSOAPGet>
{
    public override (Envelope envelope, SampleSOAPGet request) CreateEnvelope()
    {
        var envelope = new Envelope
        {
            Header = new EnvelopeHeader(),
            Body = new EnvelopeBody()
        };
        return (envelope, envelope.Body.Add);
    }

    /// <summary>
    /// Defines the envelop node in soap request
    /// </summary>
    [Serializable, XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/"), XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope : SOAPEnvelopeBase
    {
        /// <summary>
        /// add the required namespaces to envelop node. If predefined namespaces needed, then just replace ns with base.GetRequestNamespaces()
        /// </summary>
        public override string ToString()
        {
            XmlSerializerNamespaces ns = new();
            ns.Add("tem", "http://tempuri.org/");
            ns.Add("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");

            return XmlUtility.SerializeToXml(this, true, ns);
        }

        /// <summary>
        /// defines the Header node inside the envelop node
        /// </summary>
        public EnvelopeHeader Header { get; set; }

        /// <summary>
        /// Defines the Body node inside the envelop node
        /// </summary>
        public EnvelopeBody Body { get; set; }
    }

    /// <summary>
    /// In this sample the Header node is empty
    /// </summary>
    [Serializable, XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeHeader
    {

    }

    /// <summary>
    /// In this sample the Body node includes a node called Add
    /// </summary>
    [Serializable, XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeBody
    {
        [XmlElement(Namespace = "http://tempuri.org/")]
        public SampleSOAPGet Add { get; set; }
    }

    /// <summary>
    /// In this sample the Add node includes two different nodes called intA and intB
    /// </summary>
    [Serializable]
    public class SampleSOAPGet
    {
        public int intA { get; set; }
        public int intB { get; set; }
    }
}
