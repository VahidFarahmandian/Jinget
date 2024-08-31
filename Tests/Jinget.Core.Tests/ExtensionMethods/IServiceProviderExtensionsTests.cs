using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Jinget.Core.Tests._BaseData;

namespace Jinget.Core.ExtensionMethods.Tests;

[TestClass()]
public class IServiceProviderExtensionsTests
{
    IServiceProvider services;
    [TestInitialize]
    public void Initialize()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient(typeof(ISampleInterface), typeof(SampleInterfaceClass));
        services = serviceCollection.BuildServiceProvider();
    }

    [TestMethod()]
    public void should_return_registered_service()
    {
        var result = services.GetJingetService<ISampleInterface>();

        Assert.IsInstanceOfType(result, typeof(SampleInterfaceClass));
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void should_throw_exception_for_nonregistered_service() => services.GetJingetService<ITestMethod>();
}