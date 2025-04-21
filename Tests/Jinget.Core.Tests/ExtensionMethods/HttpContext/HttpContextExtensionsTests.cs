using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Jinget.Core.Attributes;
using Jinget.Core.ExtensionMethods.HttpContext;

namespace Jinget.Core.Tests.ExtensionMethods.HttpContext
{
    [TestClass]
    public class HttpContextExtensionsTests
    {
        [TestMethod]
        public void GetClaimTitlesFromEndpoints_NoEndpoints_ReturnsEmpty()
        {
            // Arrange
            var mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var mockApiDescriptionProvider = new Mock<IApiDescriptionGroupCollectionProvider>();
            mockApiDescriptionProvider.Setup(p => p.ApiDescriptionGroups).Returns(new ApiDescriptionGroupCollection([], 1));

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IApiDescriptionGroupCollectionProvider))).Returns(mockApiDescriptionProvider.Object);

            mockHttpContext.Setup(c => c.RequestServices).Returns(mockServiceProvider.Object);

            // Act
            var titles = mockHttpContext.Object.GetClaimTitlesFromEndpoints<ClaimAttribute>();

            // Assert
            Assert.IsNotNull(titles);
            Assert.AreEqual(0, titles.Count());
        }

        [TestMethod]
        public void GetClaimTitlesFromEndpoints_NoMatchingAttributes_ReturnsEmpty()
        {
            // Arrange
            var mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var mockApiDescriptionProvider = new Mock<IApiDescriptionGroupCollectionProvider>();
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    EndpointMetadata = [new object()]
                }
            };
            var apiGroup = new ApiDescriptionGroup("GroupName", [apiDescription]);
            mockApiDescriptionProvider.Setup(p => p.ApiDescriptionGroups)
                .Returns(new ApiDescriptionGroupCollection([apiGroup], 1));
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IApiDescriptionGroupCollectionProvider)))
                .Returns(mockApiDescriptionProvider.Object);
            mockHttpContext.Setup(c => c.RequestServices).Returns(mockServiceProvider.Object);

            // Act
            var titles = mockHttpContext.Object.GetClaimTitlesFromEndpoints<ClaimAttribute>();

            // Assert
            Assert.IsNotNull(titles);
            Assert.AreEqual(0, titles.Count());
        }

        [TestMethod]
        public void GetClaimTitlesFromEndpoints_SingleAttribute_ReturnsSingleTitle()
        {
            // Arrange
            var mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var mockApiDescriptionProvider = new Mock<IApiDescriptionGroupCollectionProvider>();
            var expectedTitle = "Admin";
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    EndpointMetadata = [new ClaimAttribute() { Title = expectedTitle }]
                }
            };
            var apiGroup = new ApiDescriptionGroup("GroupName", [apiDescription]);
            mockApiDescriptionProvider.Setup(p => p.ApiDescriptionGroups)
                .Returns(new ApiDescriptionGroupCollection([apiGroup], 1));
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IApiDescriptionGroupCollectionProvider)))
                .Returns(mockApiDescriptionProvider.Object);
            mockHttpContext.Setup(c => c.RequestServices).Returns(mockServiceProvider.Object);

            // Act
            var titles = mockHttpContext.Object.GetClaimTitlesFromEndpoints<ClaimAttribute>();

            // Assert
            Assert.IsNotNull(titles);
            Assert.AreEqual(1, titles.Count());
            Assert.AreEqual(expectedTitle, titles.First());
        }

        [TestMethod]
        public void GetClaimTitlesFromEndpoints_MultipleAttributes_ReturnsAllTitles()
        {
            // Arrange
            var mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var mockApiDescriptionProvider = new Mock<IApiDescriptionGroupCollectionProvider>();
            var expectedTitles = new List<string> { "Admin", "Editor" };
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    EndpointMetadata = [new ClaimAttribute() { Title = expectedTitles[0] }, new ClaimAttribute() { Title = expectedTitles[1] }]
                }
            };
            var apiGroup = new ApiDescriptionGroup("GroupName", [apiDescription]);
            mockApiDescriptionProvider.Setup(p => p.ApiDescriptionGroups)
                .Returns(new ApiDescriptionGroupCollection([apiGroup], 1));
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IApiDescriptionGroupCollectionProvider)))
                .Returns(mockApiDescriptionProvider.Object);
            mockHttpContext.Setup(c => c.RequestServices).Returns(mockServiceProvider.Object);

            // Act
            var titles = mockHttpContext.Object.GetClaimTitlesFromEndpoints<ClaimAttribute>();

            // Assert
            Assert.IsNotNull(titles);
            Assert.AreEqual(2, titles.Count());
            CollectionAssert.AreEquivalent(expectedTitles, titles.ToList());
        }

        [TestMethod]
        public void GetClaimTitlesFromEndpoints_MultipleEndpointsWithAttributes_ReturnsDistinctTitles()
        {
            // Arrange
            var mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var mockApiDescriptionProvider = new Mock<IApiDescriptionGroupCollectionProvider>();
            var expectedTitles = new List<string> { "Admin", "Editor", "Viewer" };

            var apiDescription1 = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    EndpointMetadata = [new ClaimAttribute() { Title = expectedTitles[0] }, new ClaimAttribute() { Title = expectedTitles[1] }]
                }
            };

            var apiDescription2 = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    EndpointMetadata = [new ClaimAttribute() { Title = expectedTitles[1] }, new ClaimAttribute() { Title = expectedTitles[2] }]
                }
            };

            var apiGroup = new ApiDescriptionGroup("GroupName", [apiDescription1, apiDescription2]);
            mockApiDescriptionProvider.Setup(p => p.ApiDescriptionGroups)
            .Returns(new ApiDescriptionGroupCollection([apiGroup], 1));
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IApiDescriptionGroupCollectionProvider)))
            .Returns(mockApiDescriptionProvider.Object);
            mockHttpContext.Setup(c => c.RequestServices).Returns(mockServiceProvider.Object);

            // Act
            var titles = mockHttpContext.Object.GetClaimTitlesFromEndpoints<ClaimAttribute>();

            // Assert
            Assert.IsNotNull(titles);
            Assert.AreEqual(3, titles.Count());
            CollectionAssert.AreEquivalent(expectedTitles, titles.ToList());
        }

        [TestMethod]
        public void GetClaimTitlesFromEndpoints_NullHttpContext_ThrowsArgumentNullException()
        {
            // Arrange
            Microsoft.AspNetCore.Http.HttpContext httpContext = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => httpContext.GetClaimTitlesFromEndpoints<ClaimAttribute>());
        }
    }
}