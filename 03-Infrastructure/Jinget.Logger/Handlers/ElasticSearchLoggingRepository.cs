﻿namespace Jinget.Logger.Handlers;

/// <summary>
/// Repository for Elasticsearch logging operations.
/// </summary>
public class ElasticSearchLoggingRepository(IElasticClient elasticClient, ElasticSearchSettingModel settings) : IElasticSearchLoggingRepository
{
    /// <summary>
    /// Indexes a single log entry into Elasticsearch.
    /// </summary>
    /// <param name="param">The log model to index.</param>
    /// <returns>True if the indexing was successful; otherwise, throws an exception.</returns>
    public async Task<bool> IndexAsync(LogModel param)
    {
        if (!string.IsNullOrWhiteSpace(param.PartitionKey))
        {
            if (settings.CreateIndexPerPartition)
            {
                await CreateIndexAsync(param.PartitionKey);
            }

            string indexName = GetIndexName(param.PartitionKey);
            var result = await elasticClient.IndexAsync(param, i => i.Index(indexName).Refresh(settings.RefreshType));

            if (result.IsValid)
            {
                return result.IsValid;
            }

            throw new JingetException("Jinget Says: " + result.OriginalException?.ToString());
        }

        throw new JingetException("Jinget Says: PartitionKey is null or empty");
    }

    /// <summary>
    /// Gets the index name based on the partition key and settings.
    /// </summary>
    /// <param name="partitionKey">The partition key.</param>
    /// <returns>The index name.</returns>
    string GetIndexName(string partitionKey)
    {
        string indexName = settings.CreateIndexPerPartition && !string.IsNullOrWhiteSpace(partitionKey)
            ? $"{AppDomain.CurrentDomain.FriendlyName.ToLower()}-{partitionKey.ToLower()}"
            : $"{AppDomain.CurrentDomain.FriendlyName.ToLower()}";

        var invalidChars = @" \*\\<|,>/?".ToCharArray();
        indexName = new string(indexName.Where(c => !invalidChars.Contains(c)).ToArray());

        return indexName;
    }

    /// <summary>
    /// Creates an Elasticsearch index if it doesn't exist.
    /// </summary>
    /// <param name="indexName">The name of the index to create.</param>
    /// <returns>True if the index was created or already exists; otherwise, false.</returns>
    public async Task<bool> CreateIndexAsync(string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            return false;
        }

        indexName = GetIndexName(indexName);

        if (!elasticClient.Indices.Exists(indexName.ToLower()).Exists)
        {
            var indexCreationResult = await elasticClient.Indices
                .CreateAsync(indexName.ToLower(), index => index.Map(m => m.AutoMap(typeof(LogModel)).NumericDetection(true)));

            if (!indexCreationResult.IsValid)
            {
                throw new JingetException("Jinget Says: " + indexCreationResult.OriginalException);
            }
        }

        return true;
    }

    /// <summary>
    /// Indexes a list of log entries into Elasticsearch in bulk.
    /// </summary>
    /// <param name="params">The list of log models to index.</param>
    /// <returns>True if the bulk indexing was successful; otherwise, throws an exception.</returns>
    public async Task<bool> BulkIndexAsync(IList<LogModel> @params)
    {
        foreach (var item in @params.GroupBy(g => g.PartitionKey))
        {
            if (!string.IsNullOrWhiteSpace(item.Key))
            {
                if (settings.CreateIndexPerPartition)
                {
                    await CreateIndexAsync(item.Key.ToString());
                }

                string indexName = GetIndexName(item.Key.ToString());
                var result = await elasticClient.BulkAsync(i => i.Index(indexName).CreateMany(item));

                if (!result.IsValid)
                {
                    throw new JingetException("Jinget Says: " + result.OriginalException?.ToString());
                }
            }
            else
            {
                throw new JingetException("Jinget Says: ParitionKey is null or empty");
            }
        }

        return true;
    }

    /// <summary>
    /// Retrieves the latest log entry from Elasticsearch.
    /// </summary>
    /// <param name="orderBy">Optional sorting criteria.</param>
    /// <param name="partitionKey">Optional partition key for filtering.</param>
    /// <returns>The latest log model, or null if no log entries are found.</returns>
    public async Task<LogModel?> GetLatestAsync(Func<SortDescriptor<LogModel>, IPromise<IList<ISort>>>? orderBy = null, string partitionKey = "")
    {
        string indexName = GetIndexName(partitionKey);
        var lastRecord = await elasticClient.SearchAsync<LogModel>(i =>
        {
            var expr = i.Index(indexName).From(0).Take(1).MatchAll();
            return expr.Sort(orderBy ?? (s => s.Descending(d => d.TimeStamp)));
        });

        return lastRecord.Documents.FirstOrDefault();
    }

    /// <summary>
    /// Searches for log entries in Elasticsearch based on specified criteria.
    /// </summary>
    /// <param name="partitionKey">The partition key for the search.</param>
    /// <param name="searchString">The search string.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The page size for pagination.</param>
    /// <param name="username">Optional username for filtering.</param>
    /// <param name="origin">Optional origin for filtering.</param>
    /// <returns>A list of log search view models matching the search criteria.</returns>
    public async Task<List<LogSearchViewModel>> SearchAsync(
        string partitionKey,
        string searchString,
        int pageNumber,
        int pageSize,
        string username = "",
        string origin = "")
    {
        var searchResult = await elasticClient
            .SearchAsync<LogModel>(i =>
                i.Index(GetIndexName(partitionKey))
                    .Query(x =>
                    {
                        List<QueryContainer> queryContainer = new();

                        if (!string.IsNullOrWhiteSpace(searchString))
                        {
                            queryContainer.Add(
                                x.QueryString(qs => qs.Fields(
                                    fs => fs.Fields(
                                        f => f.AdditionalData,
                                        f => f.Body,
                                        f => f.Description,
                                        f => f.Headers,
                                        f => f.IP,
                                        f => f.Method,
                                        f => f.PageUrl,
                                        f => f.PartitionKey,
                                        f => f.TraceIdentifier,
                                        f => f.SubSystem,
                                        f => f.Url,
                                        f => f.Username))
                                    .Query($"*{searchString}*")));
                        }

                        if (!string.IsNullOrWhiteSpace(username))
                        {
                            queryContainer.Add(x.Match(f => f.Field(doc => doc.Username).Query(username)));
                        }

                        return x.Bool(b =>
                        {
                            return !string.IsNullOrWhiteSpace(origin)
                                ? b
                                    .Must(queryContainer.ToArray())
                                    .MustNot(x.Match(w => w.Field(f => f.Url).Query(origin)))
                                : b.Must(queryContainer.ToArray());
                        });
                    })
                    .Aggregations(a => a.Terms("req_id", x => x.Field(f => f.TraceIdentifier.Suffix("raw"))))
                    .From((pageNumber - 1) * pageSize)
                    .Take(pageSize * 2) // because each operation consists of 1 req + 1 response
                    .Sort(s => s.Descending(d => d.TimeStamp))).ConfigureAwait(true);

        var logs = searchResult.Documents.GroupBy(x => x.TraceIdentifier);
        var result = logs.Select(l => new LogSearchViewModel(l.Key, l.Select(o => o))).ToList();

        return result.Take(pageSize).ToList();
    }
}