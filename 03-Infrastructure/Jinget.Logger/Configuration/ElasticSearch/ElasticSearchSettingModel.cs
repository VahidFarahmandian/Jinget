namespace Jinget.Logger.Configuration.ElasticSearch;

public class ElasticSearchSettingModel : BaseSettingModel
{
    /// <summary>
    /// Elasticsearch service url. This address should not contain the PROTOCOL itself. Use 'abc.com:9200' instead of 'http://abc.com:9200'
    /// </summary>
    public string Url { get; set; } = "localhost:9200";

    /// <summary>
    /// username, if basic authentication enabled on Elasticsearch search service
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// password, if basic authentication enabled on Elasticsearch search service
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Set whether to use SSL while connecting to Elasticsearch or not
    /// </summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>
    /// Set whether to validate ssl certificate or not.
    /// </summary>
    public bool BypassCertificateValidation { get; set; } = true;

    /// <summary>
    /// Create index per partition using HttpContext.Items["jinget.log.partitionkey"] value.
    /// If this mode is selected, then <see cref="RegisterDefaultLogModels"/> and also <seealso cref="DiscoveryTypes"/> will not be used.
    /// If this mode is selected, then index creation will be deferred until the first document insertion.
    /// foreach partition key, a separated index will be created. 
    /// all the indexes will share the same data model. 
    /// for request/response logs, <see cref="Entities.Log.OperationLog"/> will be used.
    /// for errors, <see cref="Entities.Log.ErrorLog"/> will be used.
    /// for custom logs, <see cref="Entities.Log.CustomLog"/> will be used.
    /// </summary>
    public bool CreateIndexPerPartition { get; set; }

    /// <summary>
    /// Control when changes made by the request are made visible to search. Default is <seealso cref="Refresh.WaitFor"/><para></para>
    /// <seealso cref="Refresh.True"/>: Refresh the relevant primary and replica shards (not the whole index) immediately after the operation occurs. Use with CAUTIONS<para></para>
    /// <seealso cref="Refresh.WaitFor"/>: Wait for the changes made by the request to be made visible by a refresh before replying. This doesn’t force an immediate refresh, rather, it waits for a refresh to happen. Elasticsearch automatically refreshes shards that have changed every index.refresh_interval which defaults to one second.<para></para>
    /// <seealso cref="Refresh.False"/>: Take no refresh related actions. The changes made by this request will be made visible at some point after the request returns.
    /// </summary>
    public Refresh RefreshType { get; set; } = Refresh.WaitFor;
}