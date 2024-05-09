using System.Collections.Generic;
using System.Threading.Tasks;
using Jinget.Logger.Entities.Log.Base;
using Jinget.Logger.ViewModels;

namespace Jinget.Logger.Handlers.CommandHandlers;

public interface IElasticSearchBaseDomainService<TModelType>
    where TModelType : LogBaseEntity
{
    Task<TModelType> FetchLatestAsync();

    Task<bool> CreateAsync(TModelType param);

    Task<bool> BulkCreateAsync(IList<TModelType> @params);
    Task<List<LogSearchViewModel>> SearchAsync(string partitionKey, string searchString, int pageNumber, int pageSize, string username = "");
}