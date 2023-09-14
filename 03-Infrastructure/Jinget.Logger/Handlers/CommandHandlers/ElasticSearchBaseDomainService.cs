using System.Collections.Generic;
using System.Threading.Tasks;
using Jinget.Logger.Entities;

namespace Jinget.Logger.Handlers.CommandHandlers
{
    public class ElasticSearchBaseDomainService<TModelType, TKeyType> :
        IElasticSearchBaseDomainService<TModelType, TKeyType>
        where TModelType : BaseEntity<TKeyType>
    {
        protected readonly IElasticSearchRepository<TModelType, TKeyType> Repository;

        public ElasticSearchBaseDomainService(IElasticSearchRepository<TModelType, TKeyType> repository) => Repository = repository;

        public virtual async Task<bool> CreateAsync(TModelType param) => await Repository.IndexAsync(param);
        public virtual async Task<bool> BulkCreateAsync(IList<TModelType> @params) => await Repository.BulkIndexAsync(@params);

        public virtual async Task<TModelType> FetchLatestAsync() => await Repository.GetLatestAsync();

        //public virtual async Task<(IReadOnlyCollection<TModelType> Records, long TotalRecordCount)> FetchAllAsync(
        //       Func<QueryContainerDescriptor<TModelType>, QueryContainer> filter = null,
        //       Func<SortDescriptor<TModelType>, IPromise<IList<ISort>>> orderBy = null,
        //       int? pageSize = 0,
        //       int? pageIndex = -1) => await Repository.QueryAsync(filter, orderBy, pageSize, pageIndex);
    }
}