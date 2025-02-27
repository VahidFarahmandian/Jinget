#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

using Jinget.Handlers.ExternalServiceHandlers.ServiceHandler.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler;

[TestClass()]
public class CustomServiceHandlerTests
{
    public class CustomServiceHandler(IServiceProvider serviceProvider, string baseUri) : JingetServiceHandler(serviceProvider, baseUri)
    {
    }

    IServiceProvider serviceProvider;
    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddTransient<JingetHttpClientFactory>();
        services.AddHttpClient("jinget-client").ConfigurePrimaryHttpMessageHandler(() => JingetHttpClientHandlerFactory.Create(true));
        serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod()]
    public async Task Should_call_get_restapiAsync()
    {
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "https://jsonplaceholder.typicode.com");
        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        customServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.AreNotEqual("", e);
        };
        customServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.IsNull(e);
        };

        var result = await customServiceHandler.GetAsync<List<SampleGetResponse>>("users");

        Assert.IsNotNull(result);
    }

    [TestMethod()]
    public async Task Should_call_post_restapiAsync()
    {
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "https://jsonplaceholder.typicode.com");
        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        customServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.AreNotEqual("", e);
        };
        customServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.IsNull(e);
        };

        var result = await customServiceHandler
            .PostAsync<SamplePostResponse>("posts",
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
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "https://jsonplaceholder.typicode.com");
        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        customServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.AreNotEqual("", e);
        };
        customServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
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

        var result = await customServiceHandler.SendAsync<SamplePutResponse>(request);

        Assert.IsNotNull(result);
    }

    [TestMethod()]
    public async Task Should_call_get_soapAsync()
    {
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "http://www.dneonline.com/calculator.asmx");
        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.IsTrue(e.IsSuccessStatusCode);
        };
        customServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.AreNotEqual("", e);
        };
        customServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.IsNull(e);
        };

        var (envelope, request) = new SampleSOAPRequest().CreateEnvelope();
        Assert.IsNotNull(envelope);
        Assert.IsNotNull(envelope.Body);
        envelope.Body.Add = new SampleSOAPRequest.SampleSOAPGet { intA = 1, intB = 2 };

        var result = await customServiceHandler.PostAsync<AddResponse>(content: envelope.ToString(), new Dictionary<string, string>
        {
            {"Content-Type","text/xml" },
            {"SOAPAction","http://tempuri.org/Add" }
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.AddResult);
    }

    //[TestMethod]
    public async Task Should_post_multipart_formdataAsync()
    {
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "https://localhost:7027/api/upload");
        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            Assert.IsTrue(e.IsSuccessStatusCode);
        };

        List<FileInfo> files = [
            new FileInfo("Sample Upload File1.txt") ,
            new FileInfo("Sample Upload File2.txt")
        ];

        var response = await customServiceHandler.UploadFilesAsync<SamplePostResponse>("something", files);
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.Status));
    }
}
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).