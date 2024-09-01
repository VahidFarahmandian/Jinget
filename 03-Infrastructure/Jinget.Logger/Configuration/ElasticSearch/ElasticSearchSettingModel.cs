using System;
using System.Collections.Generic;

namespace Jinget.Logger.Configuration.ElasticSearch;

public class ElasticSearchSettingModel
{
    /// <summary>
    /// Elasticsearch service url. This address should not contains the PROTOCOL itself. Use 'abc.com:9200' instead of 'http://abc.com:9200'
    /// </summary>
    public string Url { get; set; } = "localhost:9200";

    /// <summary>
    /// username, if basic authentication enabled on Elasticsearch search service
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// password, if basic authentication enabled on Elasticsearch search service
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Set whether to use SSL while connecting to Elasticsearch or not
    /// </summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>
    /// Set whether to validate ssl certificate or not.
    /// </summary>
    public bool BypassCertificateValidation { get; set; } = true;

    /// <summary>
    /// foreach type specified in this list, an index in Elasticsearch will be created
    /// </summary>
    public List<Type> DiscoveryTypes { get; set; }

    /// <summary>
    /// check if Elasticsearch default log models such as OperationLog, ErrorLog and CustomLog related domain services and repositories and etc
    /// should be registered in DI container or not
    /// </summary>
    public bool RegisterDefaultLogModels { get; set; }

    /// <summary>
    /// Create index per partition using HttpContext.Items["jinget.log.partitionkey"] value.
    /// If this mode is selected, then <see cref="RegisterDefaultLogModels"/> and also <seealso cref="DiscoveryTypes"/> will not be used.
    /// If this mode is selected, then index creation will be deferred until the first document insertion.
    /// foeach partition key, a separated index will be created. 
    /// all of the indexes will share the same data model. 
    /// for request/response logs, <see cref="Entities.Log.OperationLog"/> will be used.
    /// for errors, <see cref="Entities.Log.ErrorLog"/> will be used.
    /// for custom logs, <see cref="Entities.Log.CustomLog"/> will be used.
    /// </summary>
    public bool CreateIndexPerPartition { get; set; }
}