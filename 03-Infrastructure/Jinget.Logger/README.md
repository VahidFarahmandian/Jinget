# Jinget Logger
Using this library, you can easily save your application logs in Elasticsearch database or files, by calling logger.Logxxx methods.

## How to Use:

Download the package from NuGet using Package Manager:
`Install-Package Jinget.Logger`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.Logger "Here") for more information.

## Configuration

### Log to Elasticsearch: ###

`LogToElasticSearch`: By calling this method, you are going to save your logs in Elasticsearch
```csharp
builder.Host.LogToElasticSearch(blacklist);
```
`blacklist`: Log messages contain the blacklist array items will not be logged.

`minAllowedLoglevel`: Defines the minimum allowed log level. If log's severity is equal or greater than this value, then it will be saved in elasticsearch otherwise it will be ignored. If this parameter not set, then default log level will be applied(LogLevel.Information).

After setting the logging destination, you need to configure Elasticsearch:
```csharp
builder.Services.ConfigureElasticSearchLogger(
    new ElasticSearchSettingModel
    {
        CreateIndexPerPartition = <true|false>,
        UserName = <authentication username>,
        Password = <authentication password>,
        Url = <ElasticSearch Url>,
        UseSsl = <true|false>,
        UseGlobalExceptionHandler = <true|false>,
        Handle4xxResponses = <true|false>
    });
```

`Url`: Elasticsearch service url. This address should not contain the PROTOCOL itself. Use 'abc.com:9200' instead of 'http://abc.com:9200'

`UserName`: Username, if basic authentication enabled on Elasticsearch search service

`Password`: Password, if basic authentication enabled on Elasticsearch search service

`UseSsl`: Set whether to use SSL while connecting to Elasticsearch or not

`CreateIndexPerPartition`: Create index per partition using HttpContext.Items["jinget.log.partitionkey"] value. If this mode is selected, then index creation will be deferred until the first document insertion. foreach partition key, a separated index will be created. all the indexes will share the same data model.

`RefreshType`: In Elasticsearch, the Index, Update, Delete, and Bulk APIs support setting refresh to control when changes made by this request are made visible to search.

`UseGlobalExceptionHandler`: If set to true then global exception handler will be used which in turn will be rewrite the exception response output.

`Handle4xxResponses`: If set to true then http request exception handler will be used which in turn will be handle the 4xx responses.

And finally you need to add the Jinget.Logger middleware to your pipeline:
```csharp
app.UseJingetLogging();
```

If you are using partition key, then you need to set your partition key before calling `app.UseJingetLogging()`. Like below:
```csharp
app.UseWhen(p => elasticSearchSetting.CreateIndexPerPartition, appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        //define the partitioning logic
        bool partitionKeyExists = context.Request.Headers.TryGetValue("jinget.client_id", out StringValues partitionKey);
        if (partitionKeyExists)
            context.SetPartitionKey($"test.{partitionKey}");

        await next.Invoke();
    });
});
```

For example in the above code, logs will be partitioned based on the `jinget.client_id` header's value. If this header does not exist in the request, the default index name will be used which are created using the following code:
```csharp
$"{AppDomain.CurrentDomain.FriendlyName.ToLower()}";
```

When using partition key, index names will be constructed as below:

```csharp
$"{AppDomain.CurrentDomain.FriendlyName.ToLower()}-{partitionKey.ToLower()}"
```

Here is the complete configuration for a .NET Web API application:

Without Partitioning:

```csharp
using Jinget.Core.Filters;
using Jinget.Logger.Configuration;
using Jinget.Logger.Entities.Log;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var blacklist = config.GetSection("logging:BlackList").Get<string[]>();
builder.Host.LogToElasticSearch(blacklist);

var elasticSearchSetting = new ElasticSearchSettingModel
{
    UserName = "myuser",
    Password = "mypass",
    Url = "192.168.1.1:9200",
    UseSsl = false,
    UseGlobalExceptionHandler = true,
    Handle4xxResponses = false
};
builder.Services.ConfigureElasticSearchLogger(elasticSearchSetting);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseJingetLogging();
app.MapControllers();
app.Run();
```

With Partitioning:

```csharp
using Jinget.Core.Filters;
using Jinget.Logger.Configuration;
using Jinget.Logger.Configuration.Middlewares.ElasticSearch;
using Jinget.Logger.Entities.Log;
using Jinget.Logger.Handlers.CommandHandlers;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var blacklist = config.GetSection("logging:BlackList").Get<string[]>();

builder.Host.LogToElasticSearch(blacklist);
var elasticSearchSetting = new ElasticSearchSettingModel
{
    CreateIndexPerPartition = true,
    UserName = "myuser",
    Password = "mypass",
    Url = "192.168.1.1:9200",
    UseSsl = false,
    UseGlobalExceptionHandler = true,
    Handle4xxResponses = false
};
builder.Services.ConfigureElasticSearchLogger(elasticSearchSetting);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseWhen(p => elasticSearchSetting.CreateIndexPerPartition, appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        bool partitionKeyExists = context.Request.Headers.TryGetValue("jinget.client_id", out StringValues partitionKey);
        if (partitionKeyExists)
            context.SetPartitionKey($"test.{partitionKey}");

        await next.Invoke();
    });
});

app.UseJingetLogging();
app.MapControllers();
app.Run();
```

****Note****: While logging data you might need to filter the request/response headers. In order to achieve this, you can use `BlackListHeader` or `WhiteListHeader` classes.
If Both of these classes used, then only `BlackListHeader` will be applied. 
To make use of these classes you can add them to DI container like below:
for black listed headers:(headers which you do NOT want to log)
```csharp
builder.Services.Configure<BlackListHeader>(x => x.Headers = ["header1","header2"]);
```
Or for white listed headers:(headers which you want to log them ONLY)
```csharp
builder.Services.Configure<WhiteListHeader>(x => x.Headers = ["header1","header2"]);
```

---
### Log to File: ###

`LogToFile`: By calling this method, you are going to save your logs in files
```csharp
FileSettingModel fileSetting = new FileSettingModel
{
    FileNamePrefix = "Log-",
    LogDirectory = "D:\\Logs",
    RetainFileCountLimit = 5,
    FileSizeLimitMB = 10,
    UseGlobalExceptionHandler = true,
    Handle4xxResponses = true,
};
builder.Host.LogToFile(blacklist, fileSetting);
```

`blacklist`: Log messages contain the blacklist array items will not be logged.

`minAllowedLoglevel`: Defines the minimum allowed log level. Default log level is `LogLevel.Information`.

`FileNamePrefix`: Gets or sets the filename prefix to use for log files. Defaults is `logs-`

`LogDirectory`: The directory in which log files will be written, relative to the app process. Default is `Logs` directory.

`RetainedFileCountLimit`: Gets or sets a strictly positive value representing the maximum retained file count or null for no limit. Defaults is 2 files.

`FileSizeLimit`: Gets or sets a strictly positive value representing the maximum log size in MB or null for no limit. Once the log is full, no more messages will be appended. Defaults is `10MB`.

After setting the logging destination, you need to configure file logger:
```csharp
builder.Services.ConfigureFileLogger(fileSetting);
```

Here is the complete configuration for a .NET Web API application:
```csharp
using Jinget.Core.Filters;
using Jinget.Logger.Configuration;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var blacklist = config.GetSection("logging:BlackList").Get<string[]>();
FileSettingModel fileSetting = new FileSettingModel
{
    FileNamePrefix = "Log",
    LogDirectory = "Logs",
    RetainFileCountLimit = 5,
    FileSizeLimitMB = 10,
    UseGlobalExceptionHandler = true,
    Handle4xxResponses = true,
};
builder.Host.LogToFile(blacklist, fileSetting);
builder.Services.ConfigureFileLogger(fileSetting);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseJingetLogging();

app.UseAuthorization();
app.MapControllers();
app.Run();
```

---
## How to install
In order to install Jinget Logger please refer to [nuget.org](https://www.nuget.org/packages/Jinget.Logger "nuget.org")

## Contact Me
👨‍💻 Twitter: https://twitter.com/_jinget

📧 Email: farahmandian2011@gmail.com

📣 Instagram: https://www.instagram.com/vahidfarahmandian
