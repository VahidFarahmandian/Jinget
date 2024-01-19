
# Jinget.Core.DiScanner
Using this library, you can easily register your source code dependencies automatically without the need to write repetitive and annoying codes.


## How to Use:

Download the package from NuGet using Package Manager:
`Install-Package Jinget.Core.DiScanner`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.Core.DiScanner"Here") for more information.

## Methods

### Assembly Selector

|Name| Description |
|--|--|
| FromCallingAssembly | Will scan for types from the calling assembly |
| FromExecutingAssembly| Will scan for types from the currently executing assembly |
| FromEntryAssembly | Will scan for types from the entry assembly |
| FromApplicationDependencies| Will load and scan all runtime libraries referenced by the currently executing application |
| FromAssemblyDependencies| Will load and scan all runtime libraries referenced by the currently specified `assembly` |
| FromDependencyContext | Will load and scan all runtime libraries in the given `context` |
| FromAssemblyOf | Will scan for types from the assembly of type `T` |
| FromAssemblies | Will scan for types in each `Assembly` in `assemblies` |

### Service Type Selector

|Name| Description |
|--|--|
| AsSelf| Registers each matching concrete type as itself |
| FromExecutingAssembly| Will scan for types from the currently executing assembly |
| As | Registers each matching concrete type |
| AsImplementedInterfaces | Registers each matching concrete type as all of its implemented interfaces |
| AsSelfWithInterfaces | Registers each matching concrete type as all of its implemented interfaces, by returning an instance of the main type |
| AsMatchingInterface | Registers the type with the first found matching interface name.  (e.g. `ClassName` is matched to `IClassName`) |
| UsingAttributes | Registers each matching concrete type according to their `ServiceDescriptorAttribute` |

### Implementation Type Selector

|Name| Description |
|--|--|
| AddClasses | Adds all non-abstract classes from the selected assemblies to the `Microsoft.Extensions.DependencyInjection.IServiceCollection` |

### Lifetime Selector

|Name| Description |
|--|--|
| WithSingletonLifetime | Registers each matching concrete type with `ServiceLifetime.Singleton` lifetime |
| WithScopedLifetime | Registers each matching concrete type with `ServiceLifetime.Scoped` lifetime |
| WithTransientLifetime | Registers each matching concrete type with `ServiceLifetime.Transient` lifetime |
| WithLifetime | Registers each matching concrete type with the specified `lifetime` |


## Samples

> You can view more detailed sample by referring to `Jinget.CoreDiScanner.Test` project. [View UnitTests](https://github.com/VahidFarahmandian/Jinget/tree/main/Tests/Jinget.Core.DiScanner.Tests)

***Sample 1:***

Will scan for types from the calling assembly.

```csharp
_services.Scan(
    s => s.FromCallingAssembly()
        .AddClasses()
        .AsImplementedInterfaces()
        .WithTransientLifetime());
```

The above code will scan the calling assembly and registers each matching concrete type as all of its implemented interfaces. `WithTransientLifetime` indicates that the services should have `Transient` lifetime.

***Sample 2:***

Will scan for types from the calling assembly.

```csharp
_services.Scan(
    s => s.FromExecutingAssembly()
        .AddClasses()
        .AsImplementedInterfaces()
        .WithTransientLifetime());
```

The above code will scan for types from the currently executing assembly, and registers each matching concrete type as all of its implemented interfaces.

***Sample 3:***

```csharp
_services.Scan(
    s => s.FromAssembliesOf(typeof(Sample))
        .AddClasses()
        .AsSelfWithInterfaces()
        .WithSingletonLifetime());
```

The above code will scan the assembly containing the `Sample` type and registers each matching concrete type as all of its implemented interfaces, by returning an instance of the main type

***Sample 4:***

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
