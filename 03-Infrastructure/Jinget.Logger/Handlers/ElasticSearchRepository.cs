using Jinget.Core.Exceptions;
using Jinget.Logger.Configuration.Middlewares.ElasticSearch;
using Jinget.Logger.Entities.Log;
using Jinget.Logger.Entities.Log.Base;
using Jinget.Logger.ViewModels;
using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Jinget.Logger.Handlers;

public class ElasticSearchRepository<TModelType> : IElasticSearchRepository<TModelType>
    where TModelType : LogBaseEntity
{
    private readonly IElasticClient _elasticClient;
    private readonly ElasticSearchSettingModel settings;

    public ElasticSearchRepository(IElasticClient elasticClient, ElasticSearchSettingModel settings)
    {
        _elasticClient = elasticClient;
        this.settings = settings;
    }

    public async Task<bool> IndexAsync(TModelType param)
    {
        if (settings.CreateIndexPerPartition)
            await CreateIndexAsync(param.ParitionKey);
        string indexName = GetIndexName(param.ParitionKey);

        var result = await _elasticClient.IndexAsync(param, i => i.Index(indexName));
        if (result.IsValid)
            return result.IsValid;
        throw new JingetException("Jinget Says: " + result.OriginalException.ToString());
    }
    string GetIndexName(string partitionKey)
    {
        if (settings.CreateIndexPerPartition && !string.IsNullOrWhiteSpace(partitionKey))
        {
            if (typeof(TModelType) == typeof(OperationLog))
                return $"op.{partitionKey}".ToLower();
            else if (typeof(TModelType) == typeof(ErrorLog))
                return $"err.{partitionKey}".ToLower();
            else
                return $"cus.{partitionKey}".ToLower();
        }
        return $"{AppDomain.CurrentDomain.FriendlyName}.{typeof(TModelType).Name}".ToLower();
    }

    public async Task<bool> CreateIndexAsync(string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName) && typeof(TModelType) != typeof(CustomLog))
            return false;

        indexName = GetIndexName(indexName);
        if (!_elasticClient.Indices.Exists(indexName.ToLower()).Exists)
        {
            var indexCreationResult = await _elasticClient.Indices
                .CreateAsync(indexName.ToLower(), index => index.Map(m => m.AutoMap(typeof(TModelType)).NumericDetection(true)));
            if (!indexCreationResult.IsValid)
                throw new JingetException("Jinget Says: " + indexCreationResult.OriginalException);
        }
        return true;
    }

    public async Task<bool> BulkIndexAsync(IList<TModelType> @params)
    {
        foreach (var item in @params.GroupBy(g => g.ParitionKey))
        {
            if (settings.CreateIndexPerPartition)
                await CreateIndexAsync(item.Key.ToString());
            string indexName = GetIndexName(item.Key.ToString());

            var result = await _elasticClient.BulkAsync(i => i.Index(indexName).CreateMany(item));

            if (!result.IsValid)
                throw new JingetException("Jinget Says: " + result.OriginalException.ToString());
        }
        return true;
    }

    public async Task<TModelType> GetLatestAsync(Func<SortDescriptor<TModelType>, IPromise<IList<ISort>>> orderBy = null, string partitionKey = "")
    {
        string indexName = GetIndexName(partitionKey);
        var lastRecord = await _elasticClient.SearchAsync<TModelType>(i =>
        {
            var expr = i.Index(indexName).From(0).Take(1).MatchAll();
            return expr.Sort(orderBy ?? (s => s.Descending(d => d.When)));
        });

        return lastRecord.Documents.FirstOrDefault();
    }

    public async Task<List<LogSearchViewModel>> SearchAsync(
        string partitionKey,
        string searchString,
        int pageNumber,
        int pageSize,
        string username = "")
    {
        string opIndexName = "";
        string errIndexName = "";
        if (settings.CreateIndexPerPartition)
        {
            opIndexName = $"op.{partitionKey}";
            errIndexName = $"err.{partitionKey}";
        }
        if (string.IsNullOrWhiteSpace(opIndexName))
            opIndexName = $"{AppDomain.CurrentDomain.FriendlyName}.{typeof(OperationLog).Name}".ToLower();
        if (string.IsNullOrWhiteSpace(errIndexName))
            errIndexName = $"{AppDomain.CurrentDomain.FriendlyName}.{typeof(ErrorLog).Name}".ToLower();

        var searchResult = await _elasticClient
                            .SearchAsync<OperationLog>(i =>
                                    i.Index(opIndexName)
                                    .Query(x =>
                                    {
                                        List<QueryContainer> queryContainer = new();
                                        if (!string.IsNullOrWhiteSpace(searchString))
                                        {
                                            queryContainer.Add(
                                                x.MultiMatch(c => c
                                                                    .Fields(f => f
                                                                        .Field(doc => doc.Body)
                                                                        .Field(doc => doc.Description)
                                                                        .Field(doc => doc.Detail)
                                                                        .Field(doc => doc.Headers)
                                                                        .Field(doc => doc.Method)
                                                                        .Field(doc => doc.PageUrl)
                                                                        .Field(doc => doc.Url)
                                                                        .Field(doc => doc.UserName)
                                                                        .Field(doc => doc.SubSystem)
                                                                    )
                                                                .Query(searchString)
                                                            ));
                                        }
                                        if (!string.IsNullOrWhiteSpace(username))
                                            queryContainer.Add(x.Match(f => f.Field(doc => doc.UserName).Query(username)));

                                        return x.Bool(b => b.Must(queryContainer.ToArray()));
                                    })
                                    .Aggregations(a => a.Terms("req_id", x => x.Field(f => f.RequestId.Suffix("raw"))))
                                    .From((pageNumber - 1) * pageSize)
                                    .Take(pageSize * 2)//because each operation consists of 1 req + 1 response
                                    .Sort(s => s.Descending(d => d.When))).ConfigureAwait(true);

        var opResult = searchResult.Documents;
        List<LogViewModel> logs = new();

        foreach (var op in opResult.GroupBy(g => g.RequestId))
        {
            var errSearchResult = await _elasticClient.SearchAsync<ErrorLog>(i =>
            {
                var expr = i.Index(errIndexName)
                            .Query(q =>
                            {
                                List<QueryContainer> queryContainer = new()
                                {
                                    q.Match(m => m.Field(doc => doc.RequestId).Query(op.Key.ToString()))
                                };
                                return q.Bool(b => b.Must(queryContainer.ToArray()));
                            });

                return expr;
            });

            logs.Add(new LogViewModel(op, errSearchResult.Documents));
        }
        var result = logs.Select(l => new
        LogSearchViewModel(
            l.Op.First().RequestId,
            l.Op.First().IP,
            l.Op.First().Url,
            l.Op.First().Method,
            l.Op.First().PageUrl,
            l.Op.Select(o => new LogSearchViewModel.OperationLogSearchViewModel(
                o.IsResponse ? "response" : "request",
                o.When,
                o.Body,
                o.ContentLength,
                o.Headers,
                o.Detail,
                o.Description
            )),
            l.Err.Select(e => new LogSearchViewModel.ErrorLogSearchViewModel(
                e.When,
                e.Description,
                e.Severity
                ))
        )).ToList();

        return result.Take(pageSize).ToList();
    }
}