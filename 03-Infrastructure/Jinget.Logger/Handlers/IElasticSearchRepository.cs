using Jinget.Logger.Entities;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jinget.Logger.Handlers
{
    public interface IElasticSearchRepository<TModelType, TKeyType>
        where TModelType : BaseEntity<TKeyType>
    {
        Task<TModelType> GetLatestAsync(Func<SortDescriptor<TModelType>, IPromise<IList<ISort>>> orderBy = null);

        Task<bool> IndexAsync(TModelType param);

        Task<bool> BulkIndexAsync(IList<TModelType> @params);

        //Task<(IReadOnlyCollection<TModelType> Records, long TotalRecordCount)> QueryAsync(
        //    Func<QueryContainerDescriptor<TModelType>, QueryContainer> filter = null,
        //    Func<SortDescriptor<TModelType>, IPromise<IList<ISort>>> orderBy = null,
        //    int? pageSize = 0,
        //    int? pageIndex = -1);
    }
}