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

`allowedLoglevels`: Defines an array contains allowed log levels. If log's severity exists in this array, then it will be saved in elasticsearch otherwise it will be ignored. If this parameter not set, then all log levels will be allowed.

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

`CreateIndexPerPartition`: Create index per partition using HttpContext.Items["jinget.log.partitionkey"] value. If this mode is selected, then `RegisterDefaultLogModels` and also `DiscoveryTypes` will not be used. If this mode is selected, then index creation will be deferred until the first document insertion. foreach partition key, a separated index will be created. all of the indexes will share the same data model. for request/response logs, `Entities.Log.OperationLog` will be used. for errors, `Entities.Log.ErrorLog` will be used. for custom logs, `Entities.Log.CustomLog` will be used.

If you want to use partition key, instead of predefined/custom models, then you do not need to pass the generic types. Just like below:
```csharp
builder.Host.LogToElasticSearch(blacklist);
...

var elasticSearchSetting = new ElasticSearchSettingModel
{
    CreateIndexPerPartition = true,
    UserName = <authentication username>,
    Password = <authentication password>,
    Url = <ElasticSearch Url>,
    UseSsl = <true|false>
};
builder.Services.ConfigureElasticSearchLogger(elasticSearchSetting);
```

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
            context.Items.Add("jinget.log.partitionkey", $"test.{partitionKey}");

        await next.Invoke();
    });
});
```

For example in the above code, logs will be partitioned based on the `jinget.client_id` header's value. If this header does not exists in the request, the default index name will be used which are created using the following code:
```csharp
$"{AppDomain.CurrentDomain.FriendlyName}.{typeof(TModelType).Name}".ToLower();
```

When using partition key, index names will be constructed as below:

```csharp
//for operation log
$"op.{partitionKey}".ToLower();

//for error log
$"err.{partitionKey}".ToLower();

//for custom log
$"cus.{partitionKey}".ToLower();
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
builder.Host.LogToElasticSearch<OperationLog, ErrorLog, CustomLog>(blacklist);

var elasticSearchSetting = new ElasticSearchSettingModel
{
    UserName = "myuser",
    Password = "mypass",
    Url = "192.168.1.1:9200",
    UseSsl = false,
    RegisterDefaultLogModels = false,
    DiscoveryTypes = new List<Type> { typeof(OperationLog) }
};
builder.Services.ConfigureElasticSearchLogger<OperationLog, ErrorLog, CustomLog>(elasticSearchSetting);

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
    UseSsl = false
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
            context.Items.Add("jinget.log.partitionkey", $"test.{partitionKey}");

        await next.Invoke();
    });
});

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

`allowedLoglevels`: Defines an array contains allowed log levels. If log's severity exists in this array, then it will be saved in file otherwise it will be ignored. If this parameter not set, then all log levels will be allowed.

`fileNamePrefix`: Gets or sets the filename prefix to use for log files. Defaults is `logs-`

`logDirectory`: The directory in which log files will be written, relative to the app process. Default is `Logs` directory.

`retainedFileCountLimit`: Gets or sets a strictly positive value representing the maximum retained file count or null for no limit. Defaults is 2 files.

`fileSizeLimit`: Gets or sets a strictly positive value representing the maximum log size in MB or null for no limit. Once the log is full, no more messages will be appended. Defaults is `10MB`.

After setting the logging destination, you need to configure Elasticsearch:
```csharp
builder.Services.ConfigureFileLogger();
```

Here is the complete configuration for a .NET Web API application:
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
