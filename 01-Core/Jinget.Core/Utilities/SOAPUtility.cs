using AutoMapper;
using System.Xml.Serialization;

namespace Jinget.Core.Utilities
{
    public abstract class SOAPRequestBase<TEnvelope, TRequest, TViewModel>
    {
        protected readonly IMapper Mapper;

        public SOAPRequestBase() { }
        public SOAPRequestBase(IMapper mapper) => Mapper = mapper;

        public abstract (TEnvelope envelope, TRequest request) CreateEnvelope(TViewModel vm);
    }

    public abstract class SOAPRequestBase<TEnvelope, TRequest>
    {
        protected readonly IMapper Mapper;

        public SOAPRequestBase() { }
        public SOAPRequestBase(IMapper mapper) => Mapper = mapper;

        public abstract (TEnvelope envelope, TRequest request) CreateEnvelope();
    }

    public abstract class SOAPEnvelopeBase
    {
        public virtual XmlSerializerNamespaces GetRequestNamespaces()
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance/");
            ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            ns.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");

            return ns;
        }
    }
}
