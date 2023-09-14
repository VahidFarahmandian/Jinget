using System.Collections.Generic;
using System.Threading.Tasks;
using Jinget.Logger.Entities;

namespace Jinget.Logger.Handlers.CommandHandlers
{
    public interface IElasticSearchBaseDomainService<TModelType, TKeyType>
        where TModelType : BaseEntity<TKeyType>
    {
        Task<TModelType> FetchLatestAsync();

        Task<bool> CreateAsync(TModelType param);

        Task<bool> BulkCreateAsync(IList<TModelType> @params);

        //Task<(IReadOnlyCollection<TModelType> Records, long TotalRecordCount)> FetchAllAsync(
        //    Func<QueryContainerDescriptor<TModelType>, QueryContainer> filter = null,
        //    Func<SortDescriptor<TModelType>, IPromise<IList<ISort>>> orderBy = null,
        //    int? pageSize = 0,
        //    int? pageIndex = -1);
    }
}