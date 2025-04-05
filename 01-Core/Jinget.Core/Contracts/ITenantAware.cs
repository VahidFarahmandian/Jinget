namespace Jinget.Core.Contracts;

public interface ITenantAware
{
    /// <summary>
    /// Max allowed size 100 char
    /// </summary>
    string TenantId { get; set; }
}
