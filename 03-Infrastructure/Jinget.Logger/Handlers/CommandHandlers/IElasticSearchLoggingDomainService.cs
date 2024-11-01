using Jinget.ExceptionHandler.Entities.Log;

namespace Jinget.Logger.Handlers.CommandHandlers;

public interface IElasticSearchLoggingDomainService
{
    Task<LogModel> FetchLatestAsync();

    Task<bool> CreateAsync(LogModel param);

    Task<bool> BulkCreateAsync(IList<LogModel> @params);

    /// <summary></summary>
    /// <param name="partitionKey">in which index?</param>
    /// <param name="searchString">search for what?</param>
    /// <param name="pageNumber">starting from 1</param>
    /// <param name="pageSize"></param>
    /// <param name="username">filter data for this user only</param>
    /// <param name="excludeOrigin">whether to filter-out the search originated url?</param>
    /// <returns></returns>
    Task<List<LogSearchViewModel>> SearchAsync(
        string partitionKey,
        string searchString,
        int pageNumber, int pageSize,
        string username = "",
        string origin = "");
}