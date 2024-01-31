using Jinget.Core.Filters;
using Jinget.Logger.Configuration;
using Jinget.Logger.Configuration.Middlewares.ElasticSearch;
using Jinget.Logger.Entities.Log;
using Jinget.Logger.Handlers.CommandHandlers;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var blacklist = config.GetSection("logging:BlackList").Get<string[]>();

builder.Host.LogToElasticSearch(blacklist, [LogLevel.Critical, LogLevel.Error]);
var elasticSearchSetting = new ElasticSearchSettingModel
{
    CreateIndexPerPartition = true,
    UserName = "elastic",
    Password = "Aa@123456",
    Url = "localhost:9200",
    UseSsl = false
};
builder.Services.ConfigureElasticSearchLogger(elasticSearchSetting);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen().AddSwaggerGen(c => c.SchemaFilter<SwaggerExcludePropertyFilter>());

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("/logs/{search}/{page}", async (IElasticSearchBaseDomainService<OperationLog> domainService, string search, int page) =>
{
    return await domainService.SearchAsync("test.ccs", search, 1, page).ConfigureAwait(false);
});

app.Run();
