namespace Jinget.Logger.Configuration.SQLServer;

public class SQLServerSettingModel
{
    /// <summary>
    /// Connectionstring to sqlserver database.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// check if SQL Server default log models such as OperationLog, ErrorLog and CustomLog related domain services and repositories and etc
    /// should be registered in DI container or not
    /// </summary>
    public bool RegisterDefaultLogModels { get; set; }

    /// <summary>
    /// Create table per partition using HttpContext.Items["jinget.log.partitionkey"] value.
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