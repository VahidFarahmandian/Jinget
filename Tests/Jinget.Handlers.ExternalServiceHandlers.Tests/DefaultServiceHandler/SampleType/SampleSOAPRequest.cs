using Jinget.Core.Utilities;
using System.Xml.Serialization;

namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler.SampleType
{
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public EnvelopeHeader Header { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

            /// <summary>
            /// Defines the Body node inside the envelop node
            /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public EnvelopeBody Body { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public SampleSOAPGet Add { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        }

        /// <summary>
        /// In this sample the Add node includes two different nodes called intA and intB
        /// </summary>
        [Serializable]
        public class SampleSOAPGet
        {
#pragma warning disable IDE1006 // Naming Styles
            public int intA { get; set; }
            public int intB { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        }
    }
}
