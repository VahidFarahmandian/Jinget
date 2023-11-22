using AutoMapper;
using System.Xml.Serialization;

namespace Jinget.Core.Utilities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
            XmlSerializerNamespaces ns = new();
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance/");
            ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            ns.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");

            return ns;
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
