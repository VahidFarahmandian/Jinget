namespace Jinget.Logger.Handlers.CommandHandlers;

/// <summary>
/// Provides domain services for Elasticsearch logging operations.
/// </summary>
public class ElasticSearchLoggingDomainService(IElasticSearchLoggingRepository repository) : IElasticSearchLoggingDomainService
{
    // Repository for Elasticsearch logging operations.
    protected readonly IElasticSearchLoggingRepository Repository = repository;

    /// <summary>
    /// Creates a new log entry in Elasticsearch.
    /// </summary>
    /// <param name="param">The log model to create.</param>
    /// <returns>True if the log entry was created successfully; otherwise, false.</returns>
    public virtual async Task<bool> CreateAsync(LogModel param) => await Repository.IndexAsync(param);

    /// <summary>
    /// Creates multiple log entries in Elasticsearch in bulk.
    /// </summary>
    /// <param name="params">The list of log models to create.</param>
    /// <returns>True if the log entries were created successfully; otherwise, false.</returns>
    public virtual async Task<bool> BulkCreateAsync(IList<LogModel> @params) => await Repository.BulkIndexAsync(@params);

    /// <summary>
    /// Fetches the latest log entry from Elasticsearch.
    /// </summary>
    /// <returns>The latest log model, or null if no log entries are found.</returns>
    public virtual async Task<LogModel?> FetchLatestAsync() => await Repository.GetLatestAsync();

    /// <summary>
    /// Searches for log entries in Elasticsearch based on specified criteria.
    /// </summary>
    /// <param name="partitionKey">The partition key for the search.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The page size for pagination.</param>
    /// <param name="username">Optional username for filtering.</param>
    /// <param name="origin">Optional origin for filtering.</param>
    /// <returns>A list of log search view models matching the search criteria.</returns>
    public virtual async Task<List<LogSearchViewModel>> SearchAsync(
        string partitionKey,
        string searchString,
        int pageNumber, int pageSize,
        string username = "",
        string origin = "") => await Repository.SearchAsync(partitionKey, searchString, pageNumber, pageSize, username, origin);
}