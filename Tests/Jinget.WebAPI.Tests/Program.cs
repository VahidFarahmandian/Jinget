using Jinget.Core.Types;
using Jinget.Logger.Configuration.ElasticSearch;
using Jinget.Logger.Configuration.File;
using Jinget.Logger.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

string[] blacklist = ["/something"];
string[] blacklistUrl = ["/ratelimit"];

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
builder.Host.LogToFile(blacklist, fileSetting, blacklistUrl, LogLevel.Information);
builder.Services.ConfigureFileLogger(fileSetting);

//builder.Host.LogToElasticSearch(blacklist, blacklistUrl, LogLevel.Information);
//var elasticSearchSetting = new ElasticSearchSettingModel
//{
//    CreateIndexPerPartition = false,
//    UserName = "elastic",
//    Password = "UbeHc_IxSpRgZrzqsY=S",
//    Url = "localhost:9200",
//    UseSsl = false,
//    BypassCertificateValidation = true,
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
//        context.SetLoggerPartitionKey($"{DateTime.UtcNow:yyyyMMdd}");
//        await next.Invoke();
//    });
//});
app.UseWhen(p => p.Request.Path == "/detailedlog", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        context.Request.Headers.TryAdd("req-header", $"{DateTime.UtcNow:yyyyMMdd}");
        context.Request.Headers.TryAdd("AdditionalData", $"{DateTime.UtcNow:yyyyMMdd}");
        await next.Invoke();
    });
});
app.UseJingetLogging();
app.UseRouting();
app.UseCors(options =>
{
    options
    .WithOrigins("*")
    .WithMethods(["GET", "POST", "PUT", "DELETE"])
    .WithHeaders("*")
    .WithExposedHeaders("Authorization", "Content-Disposition", "Content-Type", "Content-Encoding");
    //if (middlewareConfiguration.CORSSettings.AllowedOrigins.All(x => x != "*"))
    //{
    //    options.AllowCredentials();
    //}
});
app.UseWhen(p => p.Request.Path == "/detailedlog", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        await next.Invoke();
        context.Response.Headers.TryAdd("res-header", $"{DateTime.UtcNow:yyyyMMdd}");
        context.Items.Add("AdditionalData", $"{DateTime.UtcNow:yyyyMMdd}");
    });
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ratelimit")
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.Response.WriteAsJsonAsync(new ResponseResult<ProblemDetails>(new ProblemDetails() { Detail = "Rate Limit applied" }, 0));
    }
    else
        await next.Invoke();
});

app.MapPost("/login", (LoginViewModel vm) =>
{
    if (vm.Username == "vahid")
        return new ResponseResult<string>(vm.Username, 1);
    throw new UnauthorizedAccessException();
});
app.MapGet("exception", (IHttpContextAccessor httpContextAccessor, ILogger<SampleModel> logger) =>
{
    throw new HttpRequestException("New HttpRequestException");
});
app.MapGet("customlog", (IHttpContextAccessor httpContextAccessor, ILogger<SampleModel> logger) =>
{
    //logger.LogInformation(httpContextAccessor.HttpContext, "Sample Custom message!");
    //logger.LogCustom(httpContextAccessor.HttpContext, "Sample Custom message2!");
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
public record LoginViewModel(string Username, string Password);