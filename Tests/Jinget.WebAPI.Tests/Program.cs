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

app.UseWhen(p => elasticSearchSetting.CreateIndexPerPartition, appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        context.Items.Add("jinget.log.partitionkey", $"{DateTime.Now.ToString("yyyyMMdd")}");
        await next.Invoke();
    });
});
app.UseWhen(p => p.Request.Path == "/detailedlog", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        context.Items.Add("jinget.log.partitionkey", $"{DateTime.Now.ToString("yyyyMMdd")}");
        context.Request.Headers.Add("req-header", $"{DateTime.Now.ToString("yyyyMMdd")}");
        await next.Invoke();
    });
});
app.UseJingetLogging();
app.UseWhen(p => p.Request.Path == "/detailedlog", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        await next.Invoke();
        context.Response.Headers.Add("res-header", $"{DateTime.Now.ToString("yyyyMMdd")}");
    });
});
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
    await domainService.SearchAsync("20241026", search, page, pagesize, origin: "/logs/"));

app.Run();