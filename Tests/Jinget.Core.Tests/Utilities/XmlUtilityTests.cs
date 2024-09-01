using Jinget.Core.Tests._BaseData;
using Jinget.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Jinget.Core.Tests.Utilities;

[TestClass()]
public class XmlUtilityTests
{
    XmlSample samplexml;
    SoapSample samplesoap;
    [TestInitialize()]
    public void Init()
    {
        samplexml = new()
        {
            Id = 10,
            Name = "Vahid",
            InnerSample = new()
            {
                Data = "Sample Data"
            },
            InnerSampleList =
            [
                new XmlSample.InnerXmlSample()
                {
                    Data="Sample List Data 1"
                },
                new XmlSample.InnerXmlSample()
                {
                    Data="Sample List Data 2"
                }
            ]
        };
        samplesoap = new()
        {
            Id = 10,
            Name = "Vahid"
        };
    }

    [TestMethod()]
    public void should_serialize_object_to_xml()
    {
        XmlDocument expectedXml = new();
        expectedXml.LoadXml(
        "<?xml version=\"1.0\" encoding=\"utf-16\"?><XmlSample xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>10</Id><Name>Vahid</Name><InnerSample><Data>Sample Data</Data></InnerSample><InnerSampleList><InnerXmlSample><Data>Sample List Data 1</Data></InnerXmlSample><InnerXmlSample><Data>Sample List Data 2</Data></InnerXmlSample></InnerSampleList></XmlSample>\r\n"
        );

        var result = XmlUtility.SerializeToXml(samplexml);

        XmlDocument resultXml = new();
        resultXml.LoadXml(result);

        Assert.AreEqual(expectedXml.InnerXml, resultXml.InnerXml);
    }

    [TestMethod()]
    public void should_serialize_object_to_xml_without_xmldeclartion()
    {
        XmlDocument expectedXml = new();
        expectedXml.LoadXml(
        "<XmlSample xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>10</Id><Name>Vahid</Name><InnerSample><Data>Sample Data</Data></InnerSample><InnerSampleList><InnerXmlSample><Data>Sample List Data 1</Data></InnerXmlSample><InnerXmlSample><Data>Sample List Data 2</Data></InnerXmlSample></InnerSampleList></XmlSample>\r\n"
        );

        var result = XmlUtility.SerializeToXml(samplexml, omitXmlDeclaration: true);

        XmlDocument resultXml = new();
        resultXml.LoadXml(result);

        Assert.AreEqual(expectedXml.InnerXml, resultXml.InnerXml);
    }

    [TestMethod()]
    public void should_serialize_object_to_xml_with_namespaces()
    {
        XmlDocument expectedXml = new();
        expectedXml.LoadXml(
        "<?xml version=\"1.0\" encoding=\"utf-16\"?><XmlSample xmlns:MyNS=\"https://jinget.ir\"><Id>10</Id><Name>Vahid</Name><InnerSample><Data>Sample Data</Data></InnerSample><InnerSampleList><InnerXmlSample><Data>Sample List Data 1</Data></InnerXmlSample><InnerXmlSample><Data>Sample List Data 2</Data></InnerXmlSample></InnerSampleList></XmlSample>\r\n"
        );

        XmlSerializerNamespaces ns = new();
        ns.Add("MyNS", "https://jinget.ir");

        var result = XmlUtility.SerializeToXml(samplexml, ns: ns);

        XmlDocument resultXml = new();
        resultXml.LoadXml(result);

        Assert.AreEqual(expectedXml.InnerXml, resultXml.InnerXml);
    }

    [TestMethod()]
    public void should_deserialize_xml_descendants_to_custom_type_and_return_all_of_them()
    {
        List<XmlSample.InnerXmlSample> expectedObjects =
        [
                new XmlSample.InnerXmlSample()
                {
                    Data="Sample List Data 1"
                },
                new XmlSample.InnerXmlSample()
                {
                    Data="Sample List Data 2"
                }
            ];

        string input = "<?xml version=\"1.0\" encoding=\"utf-16\"?><XmlSample xmlns:MyNS=\"https://jinget.ir\"><Id>10</Id><Name>Vahid</Name><InnerSample><Data>Sample Data</Data></InnerSample><InnerSampleList><InnerXmlSample><Data>Sample List Data 1</Data></InnerXmlSample><InnerXmlSample><Data>Sample List Data 2</Data></InnerXmlSample></InnerSampleList></XmlSample>\r\n";

        var result = XmlUtility.DeserializeXmlDescendantsAll<XmlSample.InnerXmlSample>(input);

        Assert.AreEqual(expectedObjects.Count, result.Count);
    }

    [TestMethod()]
    public void should_deserialize_xml_descendants_to_custom_type_and_return_first_among_them()
    {
        var expectedObjects = new XmlSample.InnerXmlSample()
        {
            Data = "Sample List Data 1"
        };

        string input = "<?xml version=\"1.0\" encoding=\"utf-16\"?><XmlSample xmlns:MyNS=\"https://jinget.ir\"><Id>10</Id><Name>Vahid</Name><InnerSample><Data>Sample Data</Data></InnerSample><InnerSampleList><InnerXmlSample><Data>Sample List Data 1</Data></InnerXmlSample><InnerXmlSample><Data>Sample List Data 2</Data></InnerXmlSample></InnerSampleList></XmlSample>\r\n";

        var result = XmlUtility.DeserializeXmlDescendantsFirst<XmlSample.InnerXmlSample>(input);

        Assert.AreEqual(expectedObjects.Data, result.Data);
    }

    [TestMethod()]
    public void should_serialize_object_to_soapxml()
    {
        XmlDocument expectedXml = new();
        expectedXml.LoadXml(
        "<SOAP-ENV:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:SOAP-ENC=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:clr=\"http://schemas.microsoft.com/clr/\" SOAP-ENV:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n  <SOAP-ENV:Body>\r\n    <a1:SoapSample id=\"ref-1\" xmlns:a1=\"http://schemas.microsoft.com/clr/nsassem/Jinget.Core.Tests._BaseData/Jinget.Core.Tests\">\r\n      <id>10</id>\r\n      <name id=\"ref-2\">Vahid</name>\r\n    </a1:SoapSample>\r\n  </SOAP-ENV:Body>\r\n</SOAP-ENV:Envelope>\r\n"
        );

        var result = XmlUtility.SerializeToSoapXml(samplesoap);

        XmlDocument resultXml = new();
        resultXml.LoadXml(result);

        Assert.AreEqual(expectedXml.InnerXml, resultXml.InnerXml);
    }
}