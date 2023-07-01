using System.Xml.Serialization;

namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler.SampleType
{
    /// <summary>
    /// The returned soap response should contain a node with the name same as this class
    /// </summary>
    [XmlRoot(Namespace = "http://tempuri.org/")]
    public class AddResponse
    {
        public int AddResult { get; set; }
    }
}