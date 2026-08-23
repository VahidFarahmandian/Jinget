using Jinget.Core.Enumerations;
using Jinget.Logger.Enum;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Jinget.Logger.Handlers;

/// <summary>
/// Repository for Elasticsearch logging operations.
/// </summary>
public class ElasticSearchLoggingRepository(IElasticClient elasticClient, ElasticSearchSettingModel settings) :
    IElasticSearchLoggingRepository
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
            var indexCreationResult =
         await elasticClient.Indices.CreateAsync(
             indexName,
             index => index.Map<LogModel>(m => m
                 .AutoMap()
                 .Properties(ps => ps
                     .Keyword(k => k
                         .Name(n => n.TraceIdentifier))

                     .Keyword(k => k
                         .Name(n => n.SpanIdentifier))

                     .Keyword(k => k
                         .Name(n => n.ParentSpanIdentifier))

                     .Keyword(k => k
                         .Name(n => n.RequestIdentifier))

                     .Keyword(k => k
                         .Name(n => n.PartitionKey))

                     .Keyword(k => k
                         .Name(n => n.SubSystem))

                     .Keyword(k => k
                         .Name(n => n.Username))

                     .Keyword(k => k
                         .Name(n => n.Method))

                     .Keyword(k => k
                         .Name(n => n.TypeDescription))

                     .Keyword(k => k
                         .Name(n => n.Severity))

                     .Date(d => d
                         .Name(n => n.TimeStamp))
                 )
             ));

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
    /// <param name="indexPattern">Optional index pattern for filtering.</param>
    /// <returns>The latest log model, or null if no log entries are found.</returns>
    public async Task<LogModel?> GetLatestAsync(
        Func<SortDescriptor<LogModel>, IPromise<IList<ISort>>>? orderBy = null,
        string indexPattern = "")
    {
        var indexName = indexPattern.EndsWith('*')
            ? indexPattern
            : $"{indexPattern}*";
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
    /// <param name="indexPattern">The index pattern used for searching.</param>
    /// <param name="queryString">The search string.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The page size for pagination.</param>
    /// <param name="username">Optional username for filtering.</param>
    /// <param name="origin">Optional origin for filtering.</param>
    /// <returns>A list of log search view models matching the search criteria.</returns>
    public async Task<IReadOnlyList<LogSearchViewModel>> SearchAsync(
    string indexPattern,
    int pageNumber,
    int pageSize,
    string? queryString = null,
    string? username = null,
    string? origin = null,
    CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var indexName = indexPattern.EndsWith('*')
            ? indexPattern
            : $"{indexPattern}*";

        var requiredBucketCount = checked(pageNumber * pageSize);

        // -------------------------------------------------------------
        // Build common filter
        // -------------------------------------------------------------

        Func<QueryContainerDescriptor<LogModel>, QueryContainer> buildFilter =
            q =>
            {
                var queries = new List<QueryContainer>();

                // Full text search
                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    var search = queryString.Trim();

                    queries.Add(
                        q.Bool(b => b
                            .Should(
                                sh => sh.Wildcard(w => w
                                    .Field(f => f.Url.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.PageUrl.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.Method.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.SubSystem.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.TraceIdentifier.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.RequestIdentifier.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.SpanIdentifier.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.IP.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.Username.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.Body.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive()),

                                sh => sh.Wildcard(w => w
                                    .Field(f => f.Description.Suffix("keyword"))
                                    .Value($"*{search}*")
                                    .CaseInsensitive())
                            )
                            .MinimumShouldMatch(1)));
                }

                // Exact username filtering
                if (!string.IsNullOrWhiteSpace(username))
                {
                    queries.Add(
                        q.Term(t => t
                            .Field(f => f.Username.Suffix("keyword"))
                            .Value(username)));
                }

                // URL contains origin/path
                if (!string.IsNullOrWhiteSpace(origin))
                {
                    queries.Add(
                        q.Wildcard(w => w
                            .Field(f => f.Url.Suffix("keyword"))
                            .Value($"*{origin}*")
                            .CaseInsensitive()));
                }

                return q.Bool(b => b
                    .Must(queries.ToArray()));
            };


        // -------------------------------------------------------------
        // Query 1:
        // Get page of TraceIdentifiers ordered by latest log timestamp
        // -------------------------------------------------------------

        var traceSearchResult = await elasticClient
            .SearchAsync<LogModel>(s => s
                .Index(indexName)
                .Size(0)
                .Query(buildFilter)
                .Aggregations(a => a
                    .Terms("traces", t => t
                        .Field(f => f.TraceIdentifier.Suffix("keyword"))
                        .Size(requiredBucketCount)
                        .Aggregations(aa => aa
                            .Max(
                                "latest_timestamp",
                                m => m.Field(f => f.TimeStamp)))
                        .Order(o => o
                            .Descending("latest_timestamp")))),
                cancellationToken);


        if (!traceSearchResult.IsValid)
        {
            throw new JingetException(
                "Jinget Says: " +
                traceSearchResult.OriginalException);
        }


        var traceAggregation =
            traceSearchResult.Aggregations
                .Terms("traces");


        if (traceAggregation is null)
        {
            return [];
        }


        var pageTraceIdentifiers = traceAggregation.Buckets
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.Key.ToString())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();


        if (pageTraceIdentifiers.Count == 0)
        {
            return [];
        }


        // -------------------------------------------------------------
        // Query 2:
        // Load all logs for selected TraceIdentifiers
        // -------------------------------------------------------------

        var logsSearchResult = await elasticClient
            .SearchAsync<LogModel>(s => s
                .Index(indexName)
                .Size(10_000)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            buildFilter,
                            m => m.Terms(t => t
                                .Field(f => f.TraceIdentifier.Suffix("keyword"))
                                .Terms(pageTraceIdentifiers)))))
                .Sort(sort => sort
                    .Ascending(f => f.TimeStamp)),
                cancellationToken);


        if (!logsSearchResult.IsValid)
        {
            throw new JingetException(
                "Jinget Says: " +
                logsSearchResult.OriginalException);
        }


        // -------------------------------------------------------------
        // Group logs by TraceIdentifier
        // -------------------------------------------------------------

        var logsByTraceIdentifier = logsSearchResult.Documents
            .GroupBy(x => x.TraceIdentifier)
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .ToDictionary(
                x => x.Key!,
                x => x
                    .OrderBy(l => l.TimeStamp)
                    .ToList());


        // -------------------------------------------------------------
        // Build trace-level result
        // -------------------------------------------------------------

        return pageTraceIdentifiers
            .Where(id => logsByTraceIdentifier.ContainsKey(id))
            .Select(id =>
            {
                var logs = logsByTraceIdentifier[id];

                var firstLog = logs[0];
                var lastLog = logs[^1];

                return new LogSearchViewModel(
                    TraceIdentifier: id,

                    // Request information comes from the first log
                    RequestUrl: firstLog.Url,
                    Method: firstLog.Method,
                    SubSystem: firstLog.SubSystem,

                    HopCount: logs.Count,

                    // Actual wall-clock trace duration.
                    TraceDurationMilliseconds:
                        (lastLog.TimeStamp - firstLog.TimeStamp)
                            .TotalMilliseconds,

                    // Sum of all response/request content lengths.
                    TotalContentLength:
                        logs.Sum(x => x.ContentLength),

                    StartTime:
                        firstLog.TimeStamp,

                    EndTime:
                        lastLog.TimeStamp,

                    FinalStatus:
                        GetFinalStatus(logs),

                    Logs:
                        logs);
            })
            .ToList();
    }

    private static TraceFinalStatus GetFinalStatus(IReadOnlyList<LogModel> logs)
    {
        if (logs.Count == 0)
            return TraceFinalStatus.AwaitingResponse;

        var firstLog = logs[0];

        // A response must have the same RequestIdentifier
        // as the initial request and Type == 2.
        var hasResponse = logs.Any(x =>
            x.RequestIdentifier == firstLog.RequestIdentifier &&
            x.Type == LogType.Response);

        if (!hasResponse)
            return TraceFinalStatus.AwaitingResponse;

        var latestLog = logs[^1];

        var latestSeverity =
            ParseSeverity(latestLog.TypeDescription);

        // Latest log is Error/Critical => failed.
        if (latestSeverity >= LogLevel.Error)
            return TraceFinalStatus.Failed;

        // Latest log is Information.
        if (latestSeverity == LogLevel.Information)
        {
            var hasNonInformationLog = logs.Any(x =>
                ParseSeverity(x.TypeDescription) != LogLevel.Information);

            return hasNonInformationLog
                ? TraceFinalStatus.Warning
                : TraceFinalStatus.Success;
        }

        // Latest log is Warning => warning.
        if (latestSeverity == LogLevel.Warning)
            return TraceFinalStatus.Warning;

        return TraceFinalStatus.Warning;
    }

    private static LogLevel ParseSeverity(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return LogLevel.Information;


        return System.Enum.TryParse<LogLevel>(
            value,
            ignoreCase: true,
            out var result)
            ? result
            : LogLevel.Information;
    }
}