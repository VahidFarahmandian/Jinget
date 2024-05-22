namespace Jinget.Core.Utilities.Enum;

public static class OrderByDirectionUtility
{
    /// <summary>
    /// return the <seealso cref="OrderByDirection"/> based on the given string representation
    /// </summary>
    /// <param name="direction">if value is 'asc' or 'ascending' then <seealso cref="OrderByDirection.Ascending"/>
    /// will be return otherwise <seealso cref="OrderByDirection.Descending"/> will be returned</param>
    /// <returns></returns>
    public static OrderByDirection Get(string direction)
    {
        if (string.IsNullOrWhiteSpace(direction))
            throw new ArgumentNullException(nameof(direction));
        return direction.ToLower() switch
        {
            "asc" => OrderByDirection.Ascending,
            "ascending" => OrderByDirection.Ascending,
            _ => OrderByDirection.Descending,
        };
    }
}
