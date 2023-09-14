using System;
using System.Collections.Generic;

namespace Jinget.Logger.Configuration.Middlewares.ElasticSearch
{
    public class ElasticSearchSettingModel
    {
        /// <summary>
        /// elastic search service url. If authentication is enabled, this address should not contains the PROTOCOL itself. Use 'abc.com' instead of 'http://abc.com'
        /// </summary>
        public string Url { get; set; } = "http://localhost:9200";

        /// <summary>
        /// username, if authentication enabled on elastic search service
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// password, if authentication enabled on elastic search service
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// use HTTP or HTTPS, if authentication enabled on elastic search service
        /// </summary>
        public bool UseSsl { get; set; } = false;

        /// <summary>
        /// foreach type specified in this list, an index in Elasticsearch will be created
        /// </summary>
        public List<Type> DiscoveryTypes { get; set; }

        /// <summary>
        /// check if Elasticsearch default log models such as OperationLog, ErrorLog and CustomLog related domain services and repositories and etc
        /// should be registered in DI container or not
        /// </summary>
        public bool RegisterDefaultLogModels { get; set; }
    }
}