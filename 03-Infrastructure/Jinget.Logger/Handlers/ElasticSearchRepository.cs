using Jinget.Core.Exceptions;
using Jinget.Logger.Entities;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jinget.Logger.Handlers
{
    public class ElasticSearchRepository<TModelType, TKeyType> : IElasticSearchRepository<TModelType, TKeyType>
        where TModelType : BaseEntity<TKeyType>
    {
        private readonly IElasticClient _elasticClient;

        public ElasticSearchRepository(IElasticClient elasticClient) => _elasticClient = elasticClient;

        public async Task<bool> IndexAsync(TModelType param)
        {
            string indexName = $"{AppDomain.CurrentDomain.FriendlyName}.{typeof(TModelType).Name}".ToLower();
            var result = await _elasticClient.IndexAsync(param, i => i.Index(indexName));
            if (result.IsValid)
                return result.IsValid;
            throw new JingetException("Jinget Says: " + result.OriginalException.ToString());
        }

        public async Task<bool> BulkIndexAsync(IList<TModelType> @params)
        {
            string indexName = $"{AppDomain.CurrentDomain.FriendlyName}.{typeof(TModelType).Name}".ToLower();
            var result = await _elasticClient.BulkAsync(i => i.Index(indexName).CreateMany(@params));

            if (result.IsValid)
                return result.IsValid;
            throw new JingetException("Jinget Says: " + result.OriginalException.ToString());
        }

        public async Task<TModelType> GetLatestAsync(Func<SortDescriptor<TModelType>, IPromise<IList<ISort>>> orderBy = null)
        {
            string indexName = $"{AppDomain.CurrentDomain.FriendlyName}.{typeof(TModelType).Name}".ToLower();
            var lastRecord = await _elasticClient.SearchAsync<TModelType>(i =>
            {
                var expr = i.Index(indexName).From(0).Take(1).MatchAll();
                return expr.Sort(orderBy ?? (s => s.Descending(d => d.Id)));
            });

            return lastRecord.Documents.FirstOrDefault();
        }
    }
}