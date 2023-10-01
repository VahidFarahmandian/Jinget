using Jinget.Logger.Entities.Log.Base;
using Jinget.Logger.ViewModels;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jinget.Logger.Handlers
{
    public interface IElasticSearchRepository<TModelType>
        where TModelType : LogBaseEntity
    {
        /// <summary>
        /// Create index if index not exists
        /// </summary>
        Task<bool> CreateIndexAsync(string indexName);

        /// <summary>
        /// Create a new document
        /// </summary>
        Task<bool> IndexAsync(TModelType param);

        /// <summary>
        /// Create a list of documents
        /// </summary>
        Task<bool> BulkIndexAsync(IList<TModelType> @params);

        /// <summary>
        /// Get latest log
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        Task<TModelType> GetLatestAsync(Func<SortDescriptor<TModelType>, IPromise<IList<ISort>>> orderBy = null, string partitionKey = "");

        /// <summary>
        /// Get list of logs
        /// </summary>
        /// <param name="username">If username specified, then logs will be filtered using <see cref="Entities.Log.OperationLog.UserName"/></param>
        Task<List<LogSearchViewModel>> SearchAsync(string partitionKey, string searchString, int pageNumber, int pageSize, string username = "");
    }
}