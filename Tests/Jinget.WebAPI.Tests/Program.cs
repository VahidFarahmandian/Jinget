using Jinget.ExceptionHandler.Entities;
using Jinget.ExceptionHandler.Extensions;
using Jinget.Logger.Configuration.File;
using Jinget.Logger.Extensions;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

string[] blacklist = ["/logs/"];

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
builder.Host.LogToElasticSearch(blacklist);
var elasticSearchSetting = new ElasticSearchSettingModel
{
    CreateIndexPerPartition = false,
    UserName = "elastic",
    Password = "Aa@123456",
    Url = "localhost:9200",
    UseSsl = false,
    UseGlobalExceptionHandler = true,
    Handle4xxResponses = true
};
builder.Services.ConfigureElasticSearchLogger(elasticSearchSetting);
builder.Services.AddControllers();

var app = builder.Build();

app.UseWhen(p => elasticSearchSetting.CreateIndexPerPartition, appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        context.SetLoggerPartitionKey($"{DateTime.Now:yyyyMMdd}");
        await next.Invoke();
    });
});
app.UseWhen(p => p.Request.Path == "/detailedlog", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        context.Request.Headers.Add("req-header", $"{DateTime.Now:yyyyMMdd}");
        context.Request.Headers.Add("AdditionalData", $"{DateTime.Now:yyyyMMdd}");
        await next.Invoke();
    });
});
app.UseJingetLogging();
app.UseWhen(p => p.Request.Path == "/detailedlog", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        await next.Invoke();
        context.Response.Headers.Add("res-header", $"{DateTime.Now:yyyyMMdd}");
        context.Items.Add("AdditionalData", $"{DateTime.Now:yyyyMMdd}");
    });
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
app.MapGet("successlog", () => "Hello vahid");
app.MapGet("detailedlog", () => "Sample Success");
app.MapPost("save", (BaseSettingModel setting) => setting);

app.MapGet("/logs/{search}/{page}/{pagesize}", async (
        IElasticSearchLoggingDomainService domainService, string search, int page, int pagesize) =>
    await domainService.SearchAsync("20241026", search, page, pagesize, origin: "/logs/"));

app.Run();