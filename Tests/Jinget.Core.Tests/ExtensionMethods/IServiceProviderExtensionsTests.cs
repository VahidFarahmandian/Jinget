using Microsoft.Extensions.DependencyInjection;

namespace Jinget.Core.Tests.ExtensionMethods;

[TestClass]
public class IServiceProviderExtensionsTests
{
    IServiceProvider services;
    [TestInitialize]
    public void Initialize()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ISampleInterface, SampleInterfaceClass>();
        services = serviceCollection.BuildServiceProvider();
    }

    [TestMethod]
    public void Should_return_registered_service()
    {
        var result = services.GetJingetService<ISampleInterface>();

        Assert.IsInstanceOfType<SampleInterfaceClass>(result);
    }

    [TestMethod]
    public void Should_throw_exception_for_nonregistered_service()
    {
        Assert.Throws<InvalidOperationException>(services.GetJingetService<ITestMethod>);
    }
}