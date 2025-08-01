﻿using Jinget.Core.ExtensionMethods.HttpContext;
using Microsoft.AspNetCore.Http;

namespace Jinget.Core.Tests.ExtensionMethods.HttpContext
{
    [TestClass()]
    public class HttpRequestExtensionsTests
    {
        [TestMethod()]
        public void IsPreflight_ReturnsTrue_ForValidPreflightRequest()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "OPTIONS";
            httpContext.Request.Headers.TryAdd("Origin", "https://example.com");
            httpContext.Request.Headers.TryAdd("Access-Control-Request-Method", "GET");

            Assert.IsTrue(httpContext.Request.IsPreflight());
        }
    }
}