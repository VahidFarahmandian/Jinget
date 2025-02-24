#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler;

[TestClass()]
public class JingetServiceHandlerTests
{
    [TestMethod()]
    public async Task Should_configure_httpclientfactory_by_timeoutAsync()
    {
        var jingetServiceHandler = new JingetServiceHandler<List<SampleGetResponse>>("https://jinget.ir", TimeSpan.FromSeconds(1));

        jingetServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsTrue(e.Message.Contains("Timeout of 1 seconds"));
        };

        var result = await jingetServiceHandler.GetAsync("users");

        Assert.IsNull(result);
    }
    [TestMethod()]
    public async Task Should_call_get_restapiAsync()
    {
        var jingetServiceHandler = new JingetServiceHandler<List<SampleGetResponse>>("https://jsonplaceholder.typicode.com");
        jingetServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        jingetServiceHandler.Events.RawResponseReceived += (sender, e) =>
        {
            Assert.AreNotEqual("", e);
        };
        jingetServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsNull(e);
        };
        jingetServiceHandler.Events.ResponseDeserialized += (sender, e) =>
        {
            Assert.IsNotNull(e);
        };

        var result = await jingetServiceHandler.GetAsync("users");

        Assert.IsNotNull(result);
    }

    [TestMethod()]
    public async Task Should_call_post_restapiAsync()
    {
        var jingetServiceHandler = new JingetServiceHandler<SamplePostResponse>("https://jsonplaceholder.typicode.com", true);
        jingetServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        jingetServiceHandler.Events.RawResponseReceived += (sender, e) =>
        {
            Assert.AreNotEqual("", e);
        };
        jingetServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsNull(e);
        };
        jingetServiceHandler.Events.ResponseDeserialized += (sender, e) =>
        {
            Assert.IsNotNull(e);
        };
        var result = await jingetServiceHandler
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
        var jingetServiceHandler = new JingetServiceHandler<SamplePutResponse>("https://jsonplaceholder.typicode.com", true);
        jingetServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        jingetServiceHandler.Events.RawResponseReceived += (sender, e) =>
        {
            Assert.AreNotEqual("", e);
        };
        jingetServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsNull(e);
        };
        jingetServiceHandler.Events.ResponseDeserialized += (sender, e) =>
        {
            Assert.IsNotNull(e);
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

        var result = await jingetServiceHandler.SendAsync(request);

        Assert.IsNotNull(result);
    }

    [TestMethod()]
    public async Task Should_call_get_soapAsync()
    {
        var jingetServiceHandler = new JingetServiceHandler<AddResponse>("http://www.dneonline.com/calculator.asmx");
        jingetServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        jingetServiceHandler.Events.RawResponseReceived += (sender, e) =>
        {
            Assert.AreNotEqual("", e);
        };
        jingetServiceHandler.Events.ExceptionOccurred += (sender, e) =>
        {
            Assert.IsNull(e);
        };
        jingetServiceHandler.Events.ResponseDeserialized += (sender, e) =>
        {
            Assert.IsNotNull(e);
        };

        var (envelope, request) = new SampleSOAPRequest().CreateEnvelope();
        Assert.IsNotNull(envelope);
        Assert.IsNotNull(envelope.Body);
        envelope.Body.Add = new SampleSOAPRequest.SampleSOAPGet { intA = 1, intB = 2 };

        var result = await jingetServiceHandler.PostAsync(envelope.ToString(), new Dictionary<string, string>
        {
            {"Content-Type","text/xml" },
            {"SOAPAction","http://tempuri.org/Add" }
        });

        Assert.IsFalse(result is null);
        Assert.AreEqual(3, result.AddResult);
    }

    //[TestMethod]
    public async Task Should_post_multipart_formdataAsync()
    {
        var jingetServiceHandler = new JingetServiceHandler<SamplePostResponse>("https://localhost:7027/api/upload", true);
        jingetServiceHandler.Events.ServiceCalled += (sender, e) =>
        {
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        jingetServiceHandler.Events.ResponseDeserialized += (sender, e) =>
        {
            Assert.IsNotNull(e);
        };

        List<FileInfo> files = [
            new FileInfo("Sample Upload File1.txt") ,
            new FileInfo("Sample Upload File2.txt")
        ];

        var response = await jingetServiceHandler.UploadFileAsync("something", files);
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.Status));
    }
}
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).