namespace Jinget.Core.DiScanner.Tests;

[TestClass]
public class DiScannerTest
{
    private ServiceCollection? _services;
    [TestInitialize]
    public void Initialize()
    {
        _services = [];
    }

    #region using assembly

    [TestMethod]
    public void Should_register_and_resolve_transient_di_using_calling_assembly()
    {
        _services.RemoveAll(typeof(ICustomInterface));
        _services.Scan(
            s => s.FromCallingAssembly()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_scoped_di_using_calling_assembly()
    {
        _services.RemoveAll(typeof(ICustomInterface));
        _services.Scan(
            s => s.FromCallingAssembly()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_singleton_di_using_calling_assembly()
    {
        _services.RemoveAll(typeof(ICustomInterface));
        _services.Scan(
            s => s.FromCallingAssembly()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_transient_di_using_executing_assembly()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.Scan(
            s => s.FromExecutingAssembly()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ISelector>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_scoped_di_using_executing_assembly()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.Scan(
            s => s.FromExecutingAssembly()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ISelector>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_singleton_di_using_executing_assembly()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.Scan(
            s => s.FromExecutingAssembly()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ISelector>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_transient_di_using_from_assemblies()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.RemoveAll(typeof(ICustomInterface));

        _services.Scan(
            s => s.FromAssemblies(typeof(ICustomInterface).Assembly, typeof(ISelector).Assembly)
                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService1 = provider.GetRequiredService<ISelector>();
        var resolvedService2 = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService1);
        Assert.IsNotNull(resolvedService2);
    }

    [TestMethod]
    public void Should_register_and_resolve_scoped_di_using_from_assemblies()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.RemoveAll(typeof(ICustomInterface));

        _services.Scan(
            s => s.FromAssemblies(typeof(ICustomInterface).Assembly, typeof(ISelector).Assembly).AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService1 = provider.GetRequiredService<ISelector>();
        var resolvedService2 = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService1);
        Assert.IsNotNull(resolvedService2);
    }

    [TestMethod]
    public void Should_register_and_resolve_singleton_di_using_from_assemblies()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.RemoveAll(typeof(ICustomInterface));

        _services.Scan(
            s => s.FromAssemblies(typeof(ICustomInterface).Assembly, typeof(ISelector).Assembly)
                .AddClasses()
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService1 = provider.GetRequiredService<ISelector>();
        var resolvedService2 = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService1);
        Assert.IsNotNull(resolvedService2);
    }

    [TestMethod]
    public void Should_register_and_resolve_transient_di_using_from_assembly()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.Scan(
            s => s.FromAssembliesOf(typeof(TypeSourceSelector))
                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ISelector>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_scoped_di_using_from_assembly()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.Scan(
            s => s.FromAssembliesOf(typeof(TypeSourceSelector))
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ISelector>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_singleton_di_using_from_assembly()
    {
        _services.RemoveAll(typeof(ISelector));
        _services.Scan(
            s => s.FromAssembliesOf(typeof(TypeSourceSelector))
                .AddClasses()
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ISelector>();

        Assert.IsNotNull(resolvedService);
    }

    #endregion

    #region as self

    [TestMethod]
    public void Should_register_and_resolve_di_as_self()
    {
        _services.RemoveAll(typeof(Sample));
        _services.Scan(
            s => s.FromAssembliesOf(typeof(Sample))
                .AddClasses()
                .AsSelf()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<Sample>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_di_as_custom()
    {
        _services.RemoveAll(typeof(ICustomInterface));
        _services.Scan(
            s => s.FromAssembliesOf(typeof(Sample))
                .AddClasses(x => x.AssignableTo(typeof(ICustomInterface)))
                .As<ICustomInterface>()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_di_as_self_and_impl_interfaces()
    {
        _services.RemoveAll(typeof(ICustomInterface));
        _services.RemoveAll(typeof(Sample));
        _services.Scan(
            s => s.FromAssembliesOf(typeof(Sample))
                .AddClasses()
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService1 = provider.GetRequiredService<ICustomInterface>();
        var resolvedService2 = provider.GetRequiredService<Sample>();

        Assert.IsNotNull(resolvedService1);
        Assert.IsNotNull(resolvedService2);
    }

    #endregion

    [TestMethod]
    public void Should_register_and_resolve_di__using_addtype()
    {
        _services.RemoveAll(typeof(Sample));
        _services.Scan(
            s => s.AddType<Sample>()
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService);
    }

    [TestMethod]
    public void Should_register_and_resolve_di__using_addtypes()
    {
        _services.RemoveAll(typeof(Sample));
        _services.Scan(
            s => s.AddTypes(typeof(Sample), typeof(TypeSourceSelector))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        var provider = _services.BuildServiceProvider();
        var resolvedService1 = provider.GetRequiredService<ISelector>();
        var resolvedService2 = provider.GetRequiredService<ICustomInterface>();

        Assert.IsNotNull(resolvedService1);
        Assert.IsNotNull(resolvedService2);
    }
}