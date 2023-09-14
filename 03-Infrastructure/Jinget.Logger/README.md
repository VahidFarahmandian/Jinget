# Jinget Logger
Using this library, you can easily save your application logs in Elasticsearch database or files.


## How to Use:

Download the package from NuGet using Package Manager:
`Install-Package Jinget.Logger`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.Logger "Here") for more information.

## Configuration

**Config logging destination:**

***Elasticsearch:***

`LogToElasticSearch`: By calling this method, you are going to save your logs in Elasticsearch
```csharp
builder.Host.LogToElasticSearch<OperationLog, ErrorLog, CustomLog>(blacklist);
```
`blacklist`: Log messages contain the blacklist array items will not logged.

After setting the logging destination, you need to configure Elasticsearch:
```csharp
builder.Services.ConfigureElasticSearchLogger<OperationLog, ErrorLog, CustomLog>(
    new ElasticSearchSettingModel
    {
        UserName = <authentication username>,
        Password = <authentication password>,
        Url = <ElasticSearch Url>,
        UseSsl = <true|false>,
        RegisterDefaultLogModels = <true|false>,
        DiscoveryTypes = new List<Type> { typeof(OperationLog) }
    });
```

`Url`: Elasticsearch service url. If authentication is enabled, this address should not contains the PROTOCOL itself. Use 'abc.com' instead of 'http://abc.com'

`UserName`: Username, if authentication enabled on Elasticsearch service

`Password`: Password, if authentication enabled on Elasticsearch service

`UseSsl`: Use HTTP or HTTPS, if authentication enabled on Elasticsearch service.

`RegisterDefaultLogModels`: You can configure logging using your own models instead of `OperationLog`, `ErrorLog` or `CustomLog`. In order to do so, you can simple create derived types and use them instead of these types.
When you are working with your own custom types, if you want to create index for default log models, you can set the `RegisterDefaultLogModels` property to `true`, otherwise you can set it as `false`.

`DiscoveryTypes`: Foreach type specified in this list, an index in Elasticsearch will be created

And finally you need to add the Jinget.Logger middleware to your pipeline:
```csharp
app.UseJingetLogging();
```

Here is the complete configuration for a .NET 7.0 Web API application:
```csharp
using Jinget.Core.Filters;
using Jinget.Logger.Configuration;
using Jinget.Logger.Entities.Log;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var blacklist = config.GetSection("logging:BlackList").Get<string[]>();
builder.Host.LogToElasticSearch<OperationLog, ErrorLog, CustomLog>(blacklist);

builder.Services.ConfigureElasticSearchLogger<OperationLog, ErrorLog, CustomLog>(
    new ElasticSearchSettingModel
    {
        UserName = "myuser",
        Password = "mypass",
        Url = "192.168.1.1:9200",
        UseSsl = false,
        RegisterDefaultLogModels = false,
        DiscoveryTypes = new List<Type> { typeof(OperationLog) }
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseJingetLogging();
app.MapControllers();
app.Run();
```

***File:***

`LogToFile`: By calling this method, you are going to save your logs in files
```csharp
builder.Host.LogToFile(blacklist, fileNamePrefix: "Log-", logDirectory: "D:\\logs", 10, 15);
```

`blacklist`: Log messages contain the blacklist array items will not logged.
`FileNamePrefix`: Gets or sets the filename prefix to use for log files. Defaults is `logs-`
`LogDirectory`: The directory in which log files will be written, relative to the app process. Default is `Logs` directory.
`RetainedFileCountLimit`: Gets or sets a strictly positive value representing the maximum retained file count or null for no limit. Defaults is 2 files.
`FileSizeLimit`: Gets or sets a strictly positive value representing the maximum log size in MB or null for no limit. Once the log is full, no more messages will be appended. Defaults is `10MB`.

After setting the logging destination, you need to configure Elasticsearch:
```csharp
builder.Services.ConfigureFileLogger();
```

Here is the complete configuration for a .NET 7.0 Web API application:
```csharp
using Jinget.Core.Filters;
using Jinget.Logger.Configuration;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var blacklist = config.GetSection("logging:BlackList").Get<string[]>();
builder.Host.LogToFile(blacklist, "Log-", "D:\\logs", 10, 15);
builder.Services.ConfigureFileLogger();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseJingetLogging();

app.UseAuthorization();
app.MapControllers();
app.Run();
```

------------
## How to install
In order to install Jinget Logger please refer to [nuget.org](https://www.nuget.org/packages/Jinget.Logger "nuget.org")

## Contact Me
👨‍💻 Twitter: https://twitter.com/_jinget

📧 Email: farahmandian2011@gmail.com

📣 Instagram: https://www.instagram.com/vahidfarahmandian
