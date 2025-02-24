using Jinget.Core.Types;
using Jinget.Logger.Configuration.File;
using Jinget.Logger.Extensions;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

string[] blacklist = ["/logs/"];

FileSettingModel fileSetting = new()
{
    FileNamePrefix = "Log",
    LogDirectory = "Logs",
    RetainFileCountLimit = 5,
    FileSizeLimitMB = 10,
    UseGlobalExceptionHandler = true,
    Handle4xxResponses = true,
    MaxRequestBodySize = 1024 * 1024 * 10,
    MaxResponseBodySize = 1024 * 1024 * 10
};
builder.Host.LogToFile(blacklist, fileSetting, LogLevel.Information);
builder.Services.ConfigureFileLogger(fileSetting);

//builder.Host.LogToElasticSearch(blacklist);
//var elasticSearchSetting = new ElasticSearchSettingModel
//{
//    CreateIndexPerPartition = false,
//    UserName = "elastic",
//    Password = "Aa@123456",
//    Url = "localhost:9200",
//    UseSsl = false,
//    UseGlobalExceptionHandler = true,
//    Handle4xxResponses = true,
//    MaxRequestBodySize = 1024 * 1024 * 10,
//    MaxResponseBodySize = 1024 * 1024 * 10
//};
//builder.Services.ConfigureElasticSearchLogger(elasticSearchSetting);
builder.Services.AddControllers();

var app = builder.Build();

//app.UseWhen(p => elasticSearchSetting.CreateIndexPerPartition, appBuilder =>
//{
//    appBuilder.Use(async (context, next) =>
//    {
//        context.SetLoggerPartitionKey($"{DateTime.Now:yyyyMMdd}");
//        await next.Invoke();
//    });
//});
app.UseWhen(p => p.Request.Path == "/detailedlog", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        context.Request.Headers.TryAdd("req-header", $"{DateTime.Now:yyyyMMdd}");
        context.Request.Headers.TryAdd("AdditionalData", $"{DateTime.Now:yyyyMMdd}");
        await next.Invoke();
    });
});
app.UseJingetLogging();
app.UseWhen(p => p.Request.Path == "/detailedlog", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        await next.Invoke();
        context.Response.Headers.TryAdd("res-header", $"{DateTime.Now:yyyyMMdd}");
        context.Items.Add("AdditionalData", $"{DateTime.Now:yyyyMMdd}");
    });
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ratelimit")
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.Response.WriteAsJsonAsync(new ResponseResult<string>("Rate Limit applied", 0));
    }
    else
        await next.Invoke();
});

app.MapGet("exception", (IHttpContextAccessor httpContextAccessor, ILogger<SampleModel> logger) =>
{
    throw new HttpRequestException("New HttpRequestException");
});
app.MapGet("customlog", (IHttpContextAccessor httpContextAccessor, ILogger<SampleModel> logger) =>
{
    logger.LogInformation(httpContextAccessor.HttpContext, "Sample Custom message!");
    logger.LogCustom(httpContextAccessor.HttpContext, "Sample Custom message2!");
    return "custom log saved";
});
app.MapGet("errorlog", (IHttpContextAccessor httpContextAccessor, ILogger<SampleModel> logger) =>
{
    logger.LogError(httpContextAccessor.HttpContext, "Sample Error message1!");
    logger.LogError(httpContextAccessor.HttpContext, "Sample Error message2!");
    logger.LogError(httpContextAccessor.HttpContext, "Sample Error message3!");

    throw new Exception("Sample exception");
});
app.MapGet("unauthorized", (IHttpContextAccessor httpContextAccessor, ILogger<SampleModel> logger) =>
{
    return Results.Unauthorized();
});
app.MapGet("successlog", () => "Hello vahid");
app.MapGet("detailedlog", () => "Sample Success");
app.MapPost("save", (object vm) => vm);

//app.MapGet("/logs/{search}/{page}/{pagesize}", async (
//        IElasticSearchLoggingDomainService domainService, string search, int page, int pagesize) =>
//    await domainService.SearchAsync("20241026", search, page, pagesize, origin: "/logs/"));

app.Run();