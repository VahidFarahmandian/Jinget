using Jinget.Logger;
using Jinget.Logger.Configuration;
using Jinget.Logger.Configuration.ElasticSearch;
using Jinget.Logger.Handlers.CommandHandlers;
using Jinget.WebAPI.Tests;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

string[] blacklist = ["/logs/"];

builder.Host.LogToElasticSearch(blacklist);
var elasticSearchSetting = new ElasticSearchSettingModel
{
    CreateIndexPerPartition = false,
    UserName = "elastic",
    Password = "Aa@123456",
    Url = "localhost:9200",
    UseSsl = false
};
builder.Services.ConfigureElasticSearchLogger(elasticSearchSetting);
builder.Services.AddControllers();

var app = builder.Build();

//app.UseWhen(p => elasticSearchSetting.CreateIndexPerPartition, appBuilder =>
//{
//    appBuilder.Use(async (context, next) =>
//    {
//        context.Items.Add("jinget.log.partitionkey", $"{DateTime.Now.ToString("yyyyMMdd")}");
//        await next.Invoke();
//    });
//});
//app.UseWhen(ctx => ctx.Request.Path.Value.EndsWith("detailedlog"),
//    appbuilder => appbuilder.Use(async (ctx, next) =>
//    {
//        ctx.Items["AdditionalData"] = new
//        {
//            Name = "Vahid",
//            Lastname = "Farahmandian"
//        };
//        ctx.Response.Headers.Append("AdditionalData", "some more information");
//        await next.Invoke();
//    }));
app.UseJingetLogging();

app.MapGet("customlog", (ILogger<SampleModel> logger) =>
{
    logger.LogInformation("Sample Custom message!");
    logger.LogCustom("Sample Custom message2!");
    return "custom log saved";
});
app.MapGet("errorlog", (ILogger<SampleModel> logger) => { throw new Exception("Sample Exception!"); });
app.MapGet("successlog", (ILogger<SampleModel> logger) => "Hello vahid");
app.MapGet("detailedlog", (HttpContext ctx, ILogger<SampleModel> logger) => "Sample Success");

app.MapGet("/logs/{search}/{page}/{pagesize}", async (
    HttpContext ctx,
    IElasticSearchLoggingDomainService domainService, string search, int page, int pagesize) =>
{
    return await domainService.SearchAsync("20241026", search, page, pagesize, origin: "/logs/");
});

app.Run();
