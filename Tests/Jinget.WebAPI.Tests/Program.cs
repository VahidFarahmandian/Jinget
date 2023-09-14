using Jinget.Core.Filters;
using Jinget.Logger.Configuration;
using Jinget.Logger.Configuration.Middlewares.ElasticSearch;
using Jinget.Logger.Entities.Log;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var blacklist = config.GetSection("logging:BlackList").Get<string[]>();

builder.Host.LogToElasticSearch<OperationLog, ErrorLog, CustomLog>(blacklist);
builder.Services.ConfigureElasticSearchLogger<OperationLog, ErrorLog, CustomLog>(
    new ElasticSearchSettingModel
    {
        UserName = "elastic",
        Password = "123456",
        Url = "localhost:9200",
        UseSsl = false,
        RegisterDefaultLogModels = false,
        DiscoveryTypes = new List<Type> { typeof(OperationLog) }
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen().AddSwaggerGen(c => c.SchemaFilter<SwaggerExcludePropertyFilter>());

var app = builder.Build();

app.UseJingetLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
