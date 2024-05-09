namespace Jinget.Core.IOptionTypes.ConnectionString;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BaseDBConnectionString
{
    /// <summary>
    /// queries using this connection string can change data
    /// </summary>
    public string CommandDatabase { get; set; }

    /// <summary>
    /// queries using this connection string should not change data
    /// If Always On is used, then `ApplicationIntent=ReadOnly` can be added to this connection string
    /// </summary>
    public string QueryDatabase { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

