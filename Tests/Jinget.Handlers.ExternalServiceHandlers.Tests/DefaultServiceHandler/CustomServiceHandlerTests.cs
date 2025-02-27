using Jinget.Handlers.ExternalServiceHandlers.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler;

[TestClass]
public class CustomServiceHandlerTests
{
    public class CustomServiceHandler(IServiceProvider serviceProvider, string baseUri) : JingetServiceHandler(serviceProvider, baseUri)
    {
    }

    private IServiceProvider serviceProvider;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddJingetExternalServiceHandler("jinget-client", true);
        serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnDeserializedResponse_AndTriggerEvents_WhenApiCallIsSuccessful()
    {
        // Arrange
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "https://jsonplaceholder.typicode.com");
        bool serviceCalled = false;
        bool rawResponseReceived = false;
        bool exceptionNotOccurred = true;

        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        customServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };
        customServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionNotOccurred = e == null;
        };

        // Act
        var result = await customServiceHandler.GetAsync<List<SampleGetResponse>>("users");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(serviceCalled);
        Assert.IsTrue(rawResponseReceived);
        Assert.IsTrue(exceptionNotOccurred);
    }

    [TestMethod]
    public async Task PostAsync_ShouldReturnDeserializedResponse_AndTriggerEvents_WhenApiCallIsSuccessful()
    {
        // Arrange
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "https://jsonplaceholder.typicode.com");
        bool serviceCalled = false;
        bool rawResponseReceived = false;
        bool exceptionNotOccurred = true;

        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        customServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };
        customServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionNotOccurred = e == null;
        };

        // Act
        var result = await customServiceHandler
            .PostAsync<SamplePostResponse>("posts",
                new { title = "foo", body = "bar", userId = 1 },
                new Dictionary<string, string> { { "Content-type", "application/json; charset=UTF-8" } });

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(serviceCalled);
        Assert.IsTrue(rawResponseReceived);
        Assert.IsTrue(exceptionNotOccurred);
    }

    [TestMethod]
    public async Task SendAsync_ShouldReturnDeserializedResponse_AndTriggerEvents_WhenApiCallIsSuccessful()
    {
        // Arrange
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "https://jsonplaceholder.typicode.com");
        bool serviceCalled = false;
        bool rawResponseReceived = false;
        bool exceptionNotOccurred = true;

        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        customServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };
        customServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionNotOccurred = e == null;
        };

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts/1"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonConvert.SerializeObject(new { id = 1, title = "foo", body = "bar", userId = 1 }))
        };
        request.Headers.TryAddWithoutValidation("Content-type", "application/json; charset=UTF-8");

        // Act
        var result = await customServiceHandler.SendAsync<SamplePutResponse>(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(serviceCalled);
        Assert.IsTrue(rawResponseReceived);
        Assert.IsTrue(exceptionNotOccurred);
    }

    [TestMethod]
    public async Task PostAsync_ShouldReturnDeserializedSoapResponse_AndTriggerEvents_WhenApiCallIsSuccessful()
    {
        // Arrange
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "http://www.dneonline.com/calculator.asmx");
        bool serviceCalled = false;
        bool rawResponseReceived = false;
        bool exceptionNotOccurred = true;

        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        customServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };
        customServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionNotOccurred = e == null;
        };

        var (envelope, request) = new SampleSOAPRequest().CreateEnvelope();
        Assert.IsNotNull(envelope);
        Assert.IsNotNull(envelope.Body);
        envelope.Body.Add = new SampleSOAPRequest.SampleSOAPGet { intA = 1, intB = 2 };

        // Act
        var result = await customServiceHandler.PostAsync<AddResponse>(content: envelope.ToString(), new Dictionary<string, string>
        {
            { "Content-Type", "text/xml" },
            { "SOAPAction", "http://tempuri.org/Add" }
        });

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.AddResult);
        Assert.IsTrue(serviceCalled);
        Assert.IsTrue(rawResponseReceived);
        Assert.IsTrue(exceptionNotOccurred);
    }

    //[TestMethod]
    public async Task UploadFilesAsync_ShouldReturnDeserializedResponse_AndTriggerEvents_WhenApiCallIsSuccessful()
    {
        // Arrange
        var customServiceHandler = new CustomServiceHandler(serviceProvider, "https://localhost:7027/api/upload");
        bool serviceCalled = false;

        customServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };

        List<FileInfo> files = [new FileInfo("Sample Upload File1.txt"), new FileInfo("Sample Upload File2.txt")];

        // Act
        var response = await customServiceHandler.UploadFilesAsync<SamplePostResponse>("something", files);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.Status));
        Assert.IsTrue(serviceCalled);
    }
}