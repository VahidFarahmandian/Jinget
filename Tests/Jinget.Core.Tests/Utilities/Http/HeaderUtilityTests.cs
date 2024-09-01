using Jinget.Core.Utilities.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Jinget.Core.Tests.Utilities.Http;

[TestClass()]
public class HeaderUtilityTests
{
    [TestMethod()]
    public void should_return_true_when_contenttype_exists()
    {
        Dictionary<string, string> headers = new()
        {
            { "content-type","some thing..." }
        };
        var result = HeaderUtility.HasContentType(headers);
        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void should_return_true_for_text_xml_contenttype_header()
    {
        Dictionary<string, string> headers = new()
        {
            { "content-type","text/xml" }
        };
        var result = HeaderUtility.IsXmlContentType(headers);
        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void should_return_true_for_application_xml_contenttype_header()
    {
        Dictionary<string, string> headers = new()
        {
            { "content-type","application/xml" }
        };
        var result = HeaderUtility.IsXmlContentType(headers);
        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void should_return_true_for_application_json_contenttype_header()
    {
        Dictionary<string, string> headers = new()
        {
            { "content-type","application/json" }
        };
        var result = HeaderUtility.IsJsonContentType(headers);
        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void should_return_false_for_non_xml_json_contenttype_header()
    {
        Dictionary<string, string> headers = new()
        {
            { "content-type","some thing..." }
        };
        var result = HeaderUtility.IsXmlContentType(headers);
        Assert.IsFalse(result);
    }

    [TestMethod()]
    public void should_return_contenttype_header_name()
    {
        string expectedResult = "content-type";
        Dictionary<string, string> headers = new()
        {
            { "content-type","some thing..." }
        };
        var result = HeaderUtility.GetContentTypeHeaderName(headers);
        Assert.AreEqual(result, expectedResult);
    }
}