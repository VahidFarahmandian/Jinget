using Jinget.Core.Utilities.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinget.Core.Tests.Utilities.Http;

[TestClass()]
public class MimeTypeMapTests
{
    [TestMethod()]
    public void should_try_return_valid_mimetype_for_givent_filename()
    {
        bool result = MimeTypeMap.TryGetMimeType("sample.txt", out string mimeType);
        Assert.IsTrue(result);
        Assert.AreEqual("text/plain", mimeType);
    }

    [TestMethod()]
    public void should_return_valid_mimetype_for_givent_filename()
    {
        string result = MimeTypeMap.GetMimeType("sample.txt");
        Assert.AreEqual("text/plain", result);
    }

    [TestMethod()]
    public void should_return_file_extension_for_given_mimetype()
    {
        string result = MimeTypeMap.GetExtension("text/plain");
        Assert.AreEqual(".txt", result);
    }
}