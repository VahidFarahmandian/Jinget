#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler;

[TestClass()]
public class CustomServiceHandlerTests
{
    public class CustomServiceHandler(string baseUri, bool ignoreSslErrors = false) : JingetServiceHandler(baseUri, ignoreSslErrors)
    {
    }

    [TestMethod()]
    public async Task Should_call_get_restapiAsync()
    {
        var customServiceHandler = new CustomServiceHandler("https://jsonplaceholder.typicode.com");
        customServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        customServiceHandler.Events.RawResponseReceived += (sender, e) =>
        {
            Assert.AreNotEqual("", e);
        };
        customServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsNull(e);
        };

        var result = await customServiceHandler.GetAsync("users");

        Assert.IsNotNull(result);
    }

    [TestMethod()]
    public async Task Should_call_post_restapiAsync()
    {
        var customServiceHandler = new CustomServiceHandler("https://jsonplaceholder.typicode.com");
        customServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        customServiceHandler.Events.RawResponseReceived += (sender, e) =>
        {
            Assert.AreNotEqual("", e);
        };
        customServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsNull(e);
        };

        var result = await customServiceHandler
            .PostAsync("posts",
            new
            {
                title = "foo",
                body = "bar",
                userId = 1,
            },
            new Dictionary<string, string>
            {
                {"Content-type","application/json; charset=UTF-8" }
            });

        Assert.IsNotNull(result);
    }

    [TestMethod()]
    public async Task Should_call_send_restapiAsync()
    {
        var customServiceHandler = new CustomServiceHandler("https://jsonplaceholder.typicode.com");
        customServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        customServiceHandler.Events.RawResponseReceived += (sender, e) =>
        {
            Assert.AreNotEqual("", e);
        };
        customServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsNull(e);
        };

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts/1"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                id = 1,
                title = "foo",
                body = "bar",
                userId = 1,
            }))
        };
        request.Headers.TryAddWithoutValidation("Content-type", "application/json; charset=UTF-8");

        var result = await customServiceHandler.SendAsync(request);

        Assert.IsNotNull(result);
    }

    [TestMethod()]
    public async Task Should_call_get_soapAsync()
    {
        var customServiceHandler = new CustomServiceHandler("http://www.dneonline.com/calculator.asmx");
        customServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        customServiceHandler.Events.RawResponseReceived += (sender, e) =>
        {
            Assert.AreNotEqual("", e);
        };
        customServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsNull(e);
        };

        var (envelope, request) = new SampleSOAPRequest().CreateEnvelope();
        Assert.IsNotNull(envelope);
        Assert.IsNotNull(envelope.Body);
        envelope.Body.Add = new SampleSOAPRequest.SampleSOAPGet { intA = 1, intB = 2 };

        var result = await customServiceHandler.PostAsync(envelope.ToString(), new Dictionary<string, string>
        {
            {"Content-Type","text/xml" },
            {"SOAPAction","http://tempuri.org/Add" }
        });

        Assert.IsFalse(result is null);
        Assert.AreEqual(3, XmlUtility.DeserializeXmlDescendantsFirst<AddResponse>(result).AddResult);
    }

    //[TestMethod]
    public async Task Should_post_multipart_formdataAsync()
    {
        var customServiceHandler = new CustomServiceHandler("https://localhost:7027/api/upload");
        customServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };

        List<FileInfo> files = [
            new FileInfo("Sample Upload File1.txt") ,
            new FileInfo("Sample Upload File2.txt")
        ];

        var response = await customServiceHandler.UploadFileAsync("something", files);
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(System.Text.Json.JsonSerializer.Deserialize<SamplePostResponse>(response).Status));
    }
}
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).