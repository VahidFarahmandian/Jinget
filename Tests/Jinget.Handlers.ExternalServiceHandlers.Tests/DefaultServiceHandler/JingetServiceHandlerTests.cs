using Jinget.Handlers.ExternalServiceHandlers.Tests.DefaultServiceHandler.SampleType;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Jinget.Handlers.ExternalServiceHandlers.DefaultServiceHandler.Tests
{
    [TestClass()]
    public class JingetServiceHandlerTests
    {
        [TestMethod()]
        public async Task should_call_get_restapi()
        {
            var jingetServiceHandler = new JingetServiceHandler<List<SampleGetResponse>>();
            jingetServiceHandler.Events.ServiceCalled += (object sender, HttpResponseMessage e) =>
            {
                Assert.IsTrue(e.IsSuccessStatusCode);
            };
            jingetServiceHandler.Events.RawResponseReceived += (object sender, string e) =>
            {
                Assert.IsFalse(e == "");
            };
            jingetServiceHandler.Events.ExceptionOccurred += (object sender, Exception e) =>
            {
                Assert.IsTrue(e is null);
            };
            jingetServiceHandler.Events.ResponseDeserialized += (object sender, List<SampleGetResponse> e) =>
            {
                Assert.IsFalse(e is null);
            };
            var result = await jingetServiceHandler.GetAsync("https://jsonplaceholder.typicode.com", "/users", true, null);

            Assert.IsFalse(result is null);
        }

        [TestMethod()]
        public async Task should_call_post_restapi()
        {
            var jingetServiceHandler = new JingetServiceHandler<SamplePostResponse>();
            jingetServiceHandler.Events.ServiceCalled += (object sender, HttpResponseMessage e) =>
            {
                Assert.IsTrue(e.IsSuccessStatusCode);
            };
            jingetServiceHandler.Events.RawResponseReceived += (object sender, string e) =>
            {
                Assert.IsFalse(e == "");
            };
            jingetServiceHandler.Events.ExceptionOccurred += (object sender, Exception e) =>
            {
                Assert.IsTrue(e is null);
            };
            jingetServiceHandler.Events.ResponseDeserialized += (object sender, SamplePostResponse e) =>
            {
                Assert.IsFalse(e is null);
            };
            var result = await jingetServiceHandler
                .PostAsync(
                "https://jsonplaceholder.typicode.com/posts",
                new
                {
                    title = "foo",
                    body = "bar",
                    userId = 1,
                },
                true,
                new Dictionary<string, string>
                {
                    {"Content-type","application/json; charset=UTF-8" }
                });

            Assert.IsFalse(result is null);
        }

        [TestMethod()]
        public async Task should_call_send_restapi()
        {
            var jingetServiceHandler = new JingetServiceHandler<SamplePutResponse>();
            jingetServiceHandler.Events.ServiceCalled += (object sender, HttpResponseMessage e) =>
            {
                Assert.IsTrue(e.IsSuccessStatusCode);
            };
            jingetServiceHandler.Events.RawResponseReceived += (object sender, string e) =>
            {
                Assert.IsFalse(e == "");
            };
            jingetServiceHandler.Events.ExceptionOccurred += (object sender, Exception e) =>
            {
                Assert.IsTrue(e is null);
            };
            jingetServiceHandler.Events.ResponseDeserialized += (object sender, SamplePutResponse e) =>
            {
                Assert.IsFalse(e is null);
            };

            var request = new HttpRequestMessage
            {
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

            var result = await jingetServiceHandler.SendAsync("https://jsonplaceholder.typicode.com/posts/1", request);

            Assert.IsFalse(result is null);
        }

        [TestMethod()]
        public async Task should_call_get_soap()
        {
            var jingetServiceHandler = new JingetServiceHandler<AddResponse>();
            jingetServiceHandler.Events.ServiceCalled += (object sender, HttpResponseMessage e) =>
            {
                Assert.IsTrue(e.IsSuccessStatusCode);
            };
            jingetServiceHandler.Events.RawResponseReceived += (object sender, string e) =>
            {
                Assert.IsFalse(e == "");
            };
            jingetServiceHandler.Events.ExceptionOccurred += (object sender, Exception e) =>
            {
                Assert.IsTrue(e is null);
            };
            jingetServiceHandler.Events.ResponseDeserialized += (object sender, AddResponse e) =>
            {
                Assert.IsFalse(e is null);
            };

            var (envelope, request) = new SampleSOAPRequest().CreateEnvelope();
            envelope.Body.Add = new SampleSOAPRequest.SampleSOAPGet { intA = 1, intB = 2 };

            var result = await jingetServiceHandler.PostAsync("http://www.dneonline.com/calculator.asmx", envelope.ToString(), true, new Dictionary<string, string>
            {
                {"Content-Type","text/xml" },
                {"SOAPAction","http://tempuri.org/Add" }
            });

            Assert.IsFalse(result is null);
        }
    }
}