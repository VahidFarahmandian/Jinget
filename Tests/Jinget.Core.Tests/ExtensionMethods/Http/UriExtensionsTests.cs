namespace Jinget.Core.Tests.ExtensionMethods.Http
{
    [TestClass]
    public class UriExtensionsTests
    {
        [TestMethod]
        public void should_add_querystring_to_simple_uri()
        {
            string expectedUrl = "https://jinget.ir?name=vahid&country=iran";
            var uri = new Uri($"https://jinget.ir")
                .AddQuery("name", "vahid")
                .AddQuery("country", "iran");
            Assert.AreEqual(expectedUrl, uri.ToString());
        }

        [TestMethod]
        public void should_add_querystring_to_simple_uri_and_ignore_empty_names()
        {
            string expectedUrl = "https://jinget.ir?country=iran";
            var uri = new Uri($"https://jinget.ir")
                .AddQuery("", "vahid")
                .AddQuery(null, "vahid")
                .AddQuery(" ", "vahid")
                .AddQuery("country", "iran");
            Assert.AreEqual(expectedUrl, uri.ToString());
        }

        [TestMethod]
        public void should_add_querystring_to_uri_with_path()
        {
            string expectedUrl = "https://jinget.ir/books?name=vahid&country=iran";
            var uri = new Uri($"https://jinget.ir/books")
                .AddQuery("name", "vahid")
                .AddQuery("country", "iran");
            Assert.AreEqual(expectedUrl, uri.ToString());
        }

        [TestMethod]
        public void should_add_querystring_to_uri_with_path_and_port()
        {
            string expectedUrl = "https://jinget.ir:44325/books?name=vahid&country=iran";
            var uri = new Uri($"https://jinget.ir:44325/books")
                .AddQuery("name", "vahid")
                .AddQuery("country", "iran");
            Assert.AreEqual(expectedUrl, uri.ToString());
        }

        [TestMethod]
        public void should_add_querystring_to_uri_with_path_and_port_and_prev_querystring()
        {
            string expectedUrl = "https://jinget.ir:44325/books?title=software&name=vahid&country=iran";
            var uri = new Uri($"https://jinget.ir:44325/books?title=software")
                .AddQuery("name", "vahid")
                .AddQuery("country", "iran");
            Assert.AreEqual(expectedUrl, uri.ToString());
        }
    }
}