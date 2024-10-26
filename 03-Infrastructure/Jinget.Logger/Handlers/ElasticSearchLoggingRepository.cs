namespace Jinget.Logger.Handlers;

public class ElasticSearchLoggingRepository : IElasticSearchLoggingRepository
{
    private readonly IElasticClient _elasticClient;
    private readonly ElasticSearchSettingModel settings;

    public ElasticSearchLoggingRepository(IElasticClient elasticClient, ElasticSearchSettingModel settings)
    {
        _elasticClient = elasticClient;
        this.settings = settings;
    }

    public async Task<bool> IndexAsync(LogModel param)
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
        string indexName = settings.CreateIndexPerPartition && !string.IsNullOrWhiteSpace(partitionKey)
               ? $"{AppDomain.CurrentDomain.FriendlyName.ToLower()}-{partitionKey.ToLower()}"
               : $"{AppDomain.CurrentDomain.FriendlyName.ToLower()}";
        var invalidChars = @" \*\\<|,>/?".ToCharArray();
        indexName = new(indexName.Where(c => !invalidChars.Contains(c)).ToArray());

        return indexName;
    }

    public async Task<bool> CreateIndexAsync(string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName))
            return false;

        indexName = GetIndexName(indexName);
        if (!_elasticClient.Indices.Exists(indexName.ToLower()).Exists)
        {
            var indexCreationResult = await _elasticClient.Indices
                .CreateAsync(indexName.ToLower(), index => index.Map(m => m.AutoMap(typeof(LogModel)).NumericDetection(true)));
            if (!indexCreationResult.IsValid)
                throw new JingetException("Jinget Says: " + indexCreationResult.OriginalException);
        }
        return true;
    }

    public async Task<bool> BulkIndexAsync(IList<LogModel> @params)
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

    public async Task<LogModel> GetLatestAsync(Func<SortDescriptor<LogModel>, IPromise<IList<ISort>>> orderBy = null, string partitionKey = "")
    {
        string indexName = GetIndexName(partitionKey);
        var lastRecord = await _elasticClient.SearchAsync<LogModel>(i =>
        {
            var expr = i.Index(indexName).From(0).Take(1).MatchAll();
            return expr.Sort(orderBy ?? (s => s.Descending(d => d.TimeStamp)));
        });

        return lastRecord.Documents.FirstOrDefault();
    }

    public async Task<List<LogSearchViewModel>> SearchAsync(
        string partitionKey,
        string searchString,
        int pageNumber,
        int pageSize,
        string username = "",
        string origin = "")
    {
        var searchResult = await _elasticClient
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
                                                        f => f.ParitionKey,
                                                        f => f.RequestId,
                                                        f => f.SubSystem,
                                                        f => f.Url,
                                                        f => f.Username))
                                                .Query($"*{searchString}*")));
                                        }
                                        if (!string.IsNullOrWhiteSpace(username))
                                            queryContainer.Add(x.Match(f => f.Field(doc => doc.Username).Query(username)));

                                        return x.Bool(b =>
                                        {
                                            return !string.IsNullOrWhiteSpace(origin)
                                            ? b
                                                .Must(queryContainer.ToArray())
                                                .MustNot(x.Match(w => w.Field(f => f.Url).Query(origin)))
                                            : b.Must(queryContainer.ToArray());
                                        });
                                    })
                                    .Aggregations(a => a.Terms("req_id", x => x.Field(f => f.RequestId.Suffix("raw"))))
                                    .From((pageNumber - 1) * pageSize)
                                    .Take(pageSize * 2)//because each operation consists of 1 req + 1 response
                                    .Sort(s => s.Descending(d => d.TimeStamp))).ConfigureAwait(true);

        var logs = searchResult.Documents.GroupBy(x => x.RequestId);

        var result = logs.Select(l => new LogSearchViewModel(l.Key, l.Select(o => o))).ToList();

        return result.Take(pageSize).ToList();
    }
}