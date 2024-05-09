namespace Jinget.Core.Contracts;

/// <summary>
/// Used for supporting paging mechanism in query handling
/// </summary>
public interface IPaginated
{
    /// <summary>
    /// Gets or sets the paging configuration.
    /// </summary>
    /// <value>The paging configuration.</value>
    Paging PagingConfig { get; set; }
}
