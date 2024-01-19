# Jinget.Core.DiScanner
Using this library, you can easily register your source code dependencies automatically without the need to write repetitive and annoying codes.


## How to Use:

Download the package from NuGet using Package Manager:
`Install-Package Jinget.Core.DiScanner`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.Core.DiScanner"Here") for more information.

## Samples

***FromCallingAssembly:***

Will scan for types from the calling assembly.

```csharp
_services.Scan(
    s => s.FromCallingAssembly()
        .AddClasses()
        .AsImplementedInterfaces()
        .WithTransientLifetime());
```

The above code will scan the calling assembly and registers each matching concrete type as all of its implemented interfaces. `WithTransientLifetime` indicates that the services should have `Transient` lifetime.

***FromExecutingAssembly:***

Will scan for types from the calling assembly.

```csharp
_services.Scan(
    s => s.FromExecutingAssembly()
        .AddClasses()
        .AsImplementedInterfaces()
        .WithTransientLifetime());
```

The above code will scan for types from the currently executing assembly, and registers each matching concrete type as all of its implemented interfaces.

***FromAssembliesOf:***

```csharp
_services.Scan(
    s => s.FromAssembliesOf(typeof(Sample))
        .AddClasses()
        .AsSelfWithInterfaces()
        .WithSingletonLifetime());
```

The above code will scan the assembly containing the `Sample` type and registers each matching concrete type as all of its implemented interfaces, by returning an instance of the main type

***FromAssemblies:***

```csharp
_services.Scan(
    s => s.FromAssemblies(typeof(ICustomInterface).Assembly, typeof(ISelector).Assembly)
        .AddClasses()
        .AsImplementedInterfaces()
        .WithTransientLifetime());
```

The above code will scan for types in each assembly in assemblies, and registers each matching concrete type as all of its implemented interfaces, by returning an instance of the main type

------------
## How to install
In order to install Jinget.Core.DiScanner please refer to [nuget.org](https://www.nuget.org/packages/Jinget.Core.DiScanner "nuget.org")

## Contact Me
👨‍💻 Twitter: https://twitter.com/_jinget

📧 Email: farahmandian2011@gmail.com

📣 Instagram: https://www.instagram.com/vahidfarahmandian
