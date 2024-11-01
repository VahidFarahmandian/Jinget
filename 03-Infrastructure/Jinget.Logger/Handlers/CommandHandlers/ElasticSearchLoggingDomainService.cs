namespace Jinget.Logger.Handlers.CommandHandlers;

public class ElasticSearchLoggingDomainService : IElasticSearchLoggingDomainService
{
    protected readonly IElasticSearchLoggingRepository Repository;

    public ElasticSearchLoggingDomainService(IElasticSearchLoggingRepository repository) => Repository = repository;

    public virtual async Task<bool> CreateAsync(LogModel param) => await Repository.IndexAsync(param);
    public virtual async Task<bool> BulkCreateAsync(IList<LogModel> @params) => await Repository.BulkIndexAsync(@params);
    public virtual async Task<LogModel> FetchLatestAsync() => await Repository.GetLatestAsync();
    public virtual async Task<List<LogSearchViewModel>> SearchAsync(
        string partitionKey,
        string searchString,
        int pageNumber, int pageSize,
        string username = "",
        string origin = "") => await Repository.SearchAsync(partitionKey, searchString, pageNumber, pageSize, username, origin);
}