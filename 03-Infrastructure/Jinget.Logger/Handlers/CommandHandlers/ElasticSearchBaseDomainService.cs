using System.Collections.Generic;
using System.Threading.Tasks;
using Jinget.Logger.Entities.Log.Base;
using Jinget.Logger.ViewModels;

namespace Jinget.Logger.Handlers.CommandHandlers;

public class ElasticSearchBaseDomainService<TModelType> :
    IElasticSearchBaseDomainService<TModelType>
    where TModelType : LogBaseEntity
{
    protected readonly IElasticSearchRepository<TModelType> Repository;

    public ElasticSearchBaseDomainService(IElasticSearchRepository<TModelType> repository) => Repository = repository;

    public virtual async Task<bool> CreateAsync(TModelType param) => await Repository.IndexAsync(param);
    public virtual async Task<bool> BulkCreateAsync(IList<TModelType> @params) => await Repository.BulkIndexAsync(@params);
    public virtual async Task<TModelType> FetchLatestAsync() => await Repository.GetLatestAsync();
    public virtual async Task<List<LogSearchViewModel>> SearchAsync(string partitionKey, string searchString, int pageNumber, int pageSize, string username = "") => await Repository.SearchAsync(partitionKey, searchString, pageNumber, pageSize, username);
}