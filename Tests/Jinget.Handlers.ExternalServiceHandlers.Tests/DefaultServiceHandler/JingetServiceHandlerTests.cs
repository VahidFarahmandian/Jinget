using Jinget.Handlers.ExternalServiceHandlers.Extensions;

using Microsoft.Extensions.DependencyInjection;

using System.Text.Json.Serialization;

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
    //[TestMethod]
    //public async Task should_call_json_path()
    //{
    //    var jingetServiceHandler = new JingetServiceHandler<AddResponse>(serviceProvider, "https://dev.azure.com/farahmandian/MSFarsi/");
    //    List<NewWorkItemModel> properties =
    //    [
    //        new NewWorkItemModel()
    //        {
    //            path="/fields/System.Title",
    //            value="Sample WorkItem"
    //        },
    //        new NewWorkItemModel()
    //        {
    //            path="/fields/System.Description",
    //            value="Sample description"
    //        },
    //        new NewWorkItemModel()
    //        {
    //            path="/fields/System.History",
    //            value="Sample comment"
    //        },
    //        new NewWorkItemModel()
    //        {
    //            path="/fields/System.AssignedTo",
    //            value="farahmandian2011@gmail.com"
    //        },
    //        new NewWorkItemModel()
    //        {
    //            path="/fields/System.AreaPath",
    //            value="MSFarsi"
    //        }
    //    ];
    //    var result = await jingetServiceHandler.PostAsync<NewWorkItemViewModel>("_apis/wit/workitems/$Task?api-version=7.1", 
    //        properties, 
    //        new Dictionary<string, string>
    //        {
    //            {"Content-Type","application/json-patch+json" },
    //            {"Authorization","Basic OjFueTZmTWZ4bVIwODZjVHBHeDc3NERxTnpEa3AzWlNLRU1IOHBsaXQ4RHJKTGR2VXp0TVJKUVFKOTlCR0FDQUFBQUFBQUFBQUFBQUdBWkRPM0pTSg==" },
    //        });
    //}
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
        var result = await jingetServiceHandler.PostAsync(
            requestBody: envelope.ToString(),
            requestHeaders: new Dictionary<string, string>
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
public class NewWorkItemModel
{

    public string op { get; } = "add";
    public string path { get; set; }
    public string value { get; set; }
    public string? from { get; set; } = null;
}
public class NewWorkItemViewModel
{
    public int id { get; set; }
    public int rev { get; set; }
    public Fields fields { get; set; }
    public _Links2 _links { get; set; }
    public string url { get; set; }


    public class Fields
    {
        [JsonPropertyName("System.AreaPath")]
        public string SystemAreaPath { get; set; }

        [JsonPropertyName("System.TeamProject")]
        public string SystemTeamProject { get; set; }

        [JsonPropertyName("System.IterationPath")]
        public string SystemIterationPath { get; set; }

        [JsonPropertyName("System.WorkItemType")]
        public string SystemWorkItemType { get; set; }

        [JsonPropertyName("System.State")]
        public string SystemState { get; set; }

        [JsonPropertyName("System.Reason")]
        public string SystemReason { get; set; }

        [JsonPropertyName("System.CreatedDate")]
        public DateTime SystemCreatedDate { get; set; }

        [JsonPropertyName("System.CreatedBy")]
        public SystemCreatedby SystemCreatedBy { get; set; }

        [JsonPropertyName("System.ChangedDate")]
        public DateTime SystemChangedDate { get; set; }

        [JsonPropertyName("System.ChangedBy")]
        public SystemChangedby SystemChangedBy { get; set; }

        [JsonPropertyName("System.Title")]
        public string SystemTitle { get; set; }

        [JsonPropertyName("Microsoft.VSTS.Common.StateChangeDate")]
        public DateTime MicrosoftVSTSCommonStateChangeDate { get; set; }

        [JsonPropertyName("Microsoft.VSTS.Common.Priority")]
        public int MicrosoftVSTSCommonPriority { get; set; }
    }

    public class SystemCreatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links
    {
        public Avatar avatar { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class SystemChangedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links1 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links1
    {
        public Avatar1 avatar { get; set; }
    }

    public class Avatar1
    {
        public string href { get; set; }
    }

    public class _Links2
    {
        public Self self { get; set; }
        public Workitemupdates workItemUpdates { get; set; }
        public Workitemrevisions workItemRevisions { get; set; }
        public Workitemhistory workItemHistory { get; set; }
        public Html html { get; set; }
        public Workitemtype workItemType { get; set; }
        public Fields1 fields { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Workitemupdates
    {
        public string href { get; set; }
    }

    public class Workitemrevisions
    {
        public string href { get; set; }
    }

    public class Workitemhistory
    {
        public string href { get; set; }
    }

    public class Html
    {
        public string href { get; set; }
    }

    public class Workitemtype
    {
        public string href { get; set; }
    }

    public class Fields1
    {
        public string href { get; set; }
    }

}
