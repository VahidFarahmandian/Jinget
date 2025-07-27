using System;

namespace Jinget.SourceGenerator.Common.Attributes;

/// <summary>
/// Indicates that the class should be used to generate a read model.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateWebAPIAttribute : Attribute
{
    /// <summary>
    /// Indicates that the web apis has authorization claim
    /// </summary>
    public bool HasClaim { get; set; }

    /// <summary>
    /// Indicates that the model should be tenant aware, so that all queries should have tenant filter
    /// </summary>
    public bool TenantAware { get; set; }

    /// <summary>
    /// The prefix to use for the claim.
    /// </summary>
    public string ClaimPrefix { get; set; }

    /// <summary>
    /// instead of setting claim dynamically, this value will be used. final result will be `ClaimPrefix-ClaimTitle-create/modify` etc
    /// </summary>
    public string ClaimTitle { get; set; }

    /// <summary>
    /// The claim type to use.
    /// </summary>
    public string ClaimType { get; set; }

    /// <summary>
    /// The command handler interface to use. 
    /// if interface is generic then write it >InterfaceName`NumberOfGenericArguments
    /// for example for ITest[T1,T2] write ITest`2
    /// </summary>
    public string CommandHandler { get; set; } = "IBaseCommandHandler`2";

    /// <summary>
    /// The query handler interface to use.
    /// if interface is generic then write it >InterfaceName`NumberOfGenericArguments
    /// for example for ITest[T1,T2] write ITest`2
    /// </summary>
    public string QueryHandler { get; set; } = "IBaseReadOnlyQueryHandler`2";

    /// <summary>
    /// The type to use for the trace data.
    /// if interface is generic then write it >InterfaceName`NumberOfGenericArguments
    /// for example for MyTraceData[T1,T2] write MyTraceData`2
    /// </summary>
    public string TraceDataType { get; set; }

    /// <summary>
    /// The type to use for the user context.
    /// if interface is generic then write it >InterfaceName`NumberOfGenericArguments
    /// for example for MyTraceData[T1,T2] write MyTraceData`2
    /// </summary>
    public string UserContextType { get; set; }

    /// <summary>
    /// The type to use for the model.
    /// if not specified, then model will be constructed using model in handlers
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// The type to use for the readonly model.
    /// if not specified, then readonly model will be constructed using model in handlers
    /// </summary>
    public string ReadOnlyModel { get; set; }
}
