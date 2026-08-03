namespace Jinget.Logger.Handlers.CommandHandlers;

public interface IElasticSearchLoggingDomainService
{
    Task<LogModel?> FetchLatestAsync();

    Task<bool> CreateAsync(LogModel param);

    Task<bool> BulkCreateAsync(IList<LogModel> @params);

    /// <summary></summary>
    /// <param name="indexPattern">in which index?</param>
    /// <param name="queryString">search for what?</param>
    /// <param name="pageNumber">starting from 1</param>
    /// <param name="pageSize"></param>
    /// <param name="username">filter data for this user only</param>
    /// <param name="origin">specific filter for url</param>
    /// <returns></returns>
    Task<List<LogSearchViewModel>> SearchAsync(
        string indexPattern,
        string? queryString,
        int pageNumber, int pageSize,
        string username = "",
        string origin = "");
}