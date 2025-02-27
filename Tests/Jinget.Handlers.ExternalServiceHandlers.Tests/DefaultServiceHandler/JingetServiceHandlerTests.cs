using Jinget.Handlers.ExternalServiceHandlers.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler;

[TestClass()]
public class JingetServiceHandlerTests
{
    private IServiceProvider serviceProvider;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddJingetExternalServiceHandler("jinget-client", true);
        serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task GetAsync_ShouldNotThrowSslException_WhenSslErrorsAreIgnored()
    {
        // Arrange
        var jingetServiceHandler = new JingetServiceHandler<List<SampleGetResponse>>(serviceProvider, "https://webmail.jinget.ir");
        bool exceptionOccurred = false;

        jingetServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionOccurred = e != null && e.Message.Contains("The SSL connection could not be established, see inner exception.");
        };
        try
        {
            // Act
            var result = await jingetServiceHandler.GetAsync("users");

            //Assert
            Assert.IsNull(result);
        }
        catch { }
        Assert.IsFalse(exceptionOccurred);
    }

    [TestMethod]
    [ExpectedException(typeof(TaskCanceledException))]
    public async Task GetAsync_ShouldThrowTimeoutException_WhenTimeoutIsConfigured()
    {
        // Arrange
        var jingetServiceHandler = new JingetServiceHandler<List<SampleGetResponse>>(serviceProvider, "https://jinget.ir", timeout: TimeSpan.FromSeconds(1));
        bool timeoutExceptionOccurred = false;

        jingetServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            timeoutExceptionOccurred = e != null && e.Message.Contains("Timeout of 1 seconds");
        };

        // Act
        var result = await jingetServiceHandler.GetAsync("users");

        //// Assert
        //Assert.IsNull(result);
        //Assert.IsTrue(timeoutExceptionOccurred);
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnDeserializedResponse_AndTriggerEvents_WhenApiCallIsSuccessful()
    {
        // Arrange
        var jingetServiceHandler = new JingetServiceHandler<List<SampleGetResponse>>(serviceProvider, "https://jsonplaceholder.typicode.com");
        bool serviceCalled = false;
        bool rawResponseReceived = false;
        bool exceptionNotOccurred = true;
        bool responseDeserialized = false;

        jingetServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        jingetServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };
        jingetServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionNotOccurred = e == null;
        };
        jingetServiceHandler.Events.ResponseDeserializedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            responseDeserialized = e != null;
        };

        // Act
        var result = await jingetServiceHandler.GetAsync("users");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(serviceCalled);
        Assert.IsTrue(rawResponseReceived);
        Assert.IsTrue(exceptionNotOccurred);
        Assert.IsTrue(responseDeserialized);
    }

    [TestMethod]
    public async Task PostAsync_ShouldReturnDeserializedResponse_AndTriggerEvents_WhenApiCallIsSuccessful()
    {
        // Arrange
        var jingetServiceHandler = new JingetServiceHandler<SamplePostResponse>(serviceProvider, "https://jsonplaceholder.typicode.com");
        bool serviceCalled = false;
        bool rawResponseReceived = false;
        bool exceptionNotOccurred = true;
        bool responseDeserialized = false;

        jingetServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        jingetServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };
        jingetServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionNotOccurred = e == null;
        };
        jingetServiceHandler.Events.ResponseDeserializedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            responseDeserialized = e != null;
        };

        // Act
        var result = await jingetServiceHandler
            .PostAsync("posts",
                new { title = "foo", body = "bar", userId = 1 },
                new Dictionary<string, string> { { "Content-type", "application/json; charset=UTF-8" } });

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(serviceCalled);
        Assert.IsTrue(rawResponseReceived);
        Assert.IsTrue(exceptionNotOccurred);
        Assert.IsTrue(responseDeserialized);
    }

    [TestMethod]
    public async Task SendAsync_ShouldReturnHttpResponseMessage_AndTriggerEvents_WhenApiCallIsSuccessful()
    {
        // Arrange
        var jingetServiceHandler = new JingetServiceHandler<SamplePutResponse>(serviceProvider, "https://jsonplaceholder.typicode.com");
        bool serviceCalled = false;
        bool rawResponseReceived = false;
        bool exceptionNotOccurred = true;
        bool responseDeserialized = false;

        jingetServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        jingetServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };
        jingetServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionNotOccurred = e == null;
        };
        jingetServiceHandler.Events.ResponseDeserializedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            responseDeserialized = e != null;
        };

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts/1"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonConvert.SerializeObject(new { id = 1, title = "foo", body = "bar", userId = 1 }))
        };
        request.Headers.TryAddWithoutValidation("Content-type", "application/json; charset=UTF-8");

        // Act
        var result = await jingetServiceHandler.SendAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(serviceCalled);
        Assert.IsTrue(rawResponseReceived);
        Assert.IsTrue(exceptionNotOccurred);
        Assert.IsTrue(responseDeserialized);
    }

    [TestMethod]
    public async Task PostAsync_ShouldDeserializeSoapResponse_AndTriggerEvents_WhenSoapCallIsSuccessful()
    {
        // Arrange
        var jingetServiceHandler = new JingetServiceHandler<AddResponse>(serviceProvider, "http://www.dneonline.com/calculator.asmx");
        bool serviceCalled = false;
        bool rawResponseReceived = false;
        bool exceptionNotOccurred = true;
        bool responseDeserialized = false;

        jingetServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        jingetServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };
        jingetServiceHandler.Events.ExceptionOccurredAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            exceptionNotOccurred = e == null;
        };
        jingetServiceHandler.Events.ResponseDeserializedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            responseDeserialized = e != null;
        };

        var (envelope, request) = new SampleSOAPRequest().CreateEnvelope();
        Assert.IsNotNull(envelope);
        Assert.IsNotNull(envelope.Body);
        envelope.Body.Add = new SampleSOAPRequest.SampleSOAPGet { intA = 1, intB = 2 };

        // Act
        var result = await jingetServiceHandler.PostAsync(envelope.ToString(), new Dictionary<string, string>
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
        Assert.IsTrue(responseDeserialized);
    }

    //[TestMethod]
    public async Task UploadFilesAsync_ShouldReturnDeserializedResponse_AndTriggerEvents_WhenMultipartUploadIsSuccessful()
    {
        // Arrange
        var jingetServiceHandler = new JingetServiceHandler<SamplePostResponse>(serviceProvider, "https://localhost:7027/api/upload");
        bool serviceCalled = false;
        bool rawResponseReceived = false;

        jingetServiceHandler.Events.ServiceCalledAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            serviceCalled = e.IsSuccessStatusCode;
        };
        jingetServiceHandler.Events.RawResponseReceivedAsync += async (sender, e) =>
        {
            await Task.CompletedTask;
            rawResponseReceived = !string.IsNullOrEmpty(e);
        };

        List<FileInfo> files = [
            new FileInfo("Sample Upload File1.txt"),
        new FileInfo("Sample Upload File2.txt")
        ];

        // Act
        var response = await jingetServiceHandler.UploadFilesAsync("something", files);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.Status));
        Assert.IsTrue(serviceCalled);
        Assert.IsTrue(rawResponseReceived);
    }
}