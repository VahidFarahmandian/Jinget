using Jinget.ExceptionHandler.Entities.Log;

namespace Jinget.Logger.Handlers;

public interface IElasticSearchLoggingRepository
{
    /// <summary>
    /// Create index if index not exists
    /// </summary>
    Task<bool> CreateIndexAsync(string indexName);

    /// <summary>
    /// Create a new document
    /// </summary>
    Task<bool> IndexAsync(LogModel param);

    /// <summary>
    /// Create a list of documents
    /// </summary>
    Task<bool> BulkIndexAsync(IList<LogModel> @params);

    /// <summary>
    /// Get latest log
    /// </summary>
    /// <param name="orderBy"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    Task<LogModel> GetLatestAsync(Func<SortDescriptor<LogModel>, IPromise<IList<ISort>>> orderBy = null, string partitionKey = "");

    /// <summary>
    /// Get list of logs
    /// </summary>
    /// <param name="username">If username specified, then logs will be filtered using <see cref="Entities.Log.OperationLog.UserName"/></param>
    Task<List<LogSearchViewModel>> SearchAsync(
        string partitionKey,
        string searchString,
        int pageNumber, int pageSize,
        string username = "",
        string origin = "");
}