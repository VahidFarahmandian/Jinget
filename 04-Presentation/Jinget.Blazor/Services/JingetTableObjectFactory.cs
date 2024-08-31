using Jinget.Core.Enumerations;

namespace Jinget.Blazor.Services;

public class JingetTableModel<T>
{
    public ICollection<T> Items { get; set; }
    public int TotalItems { get; set; }
}
public class JingetTableObjectFactory<T>
{
    public sealed class EmptyTableData : JingetTableModel<T>
    {
        static readonly Lazy<EmptyTableData> lazy = new(() => new EmptyTableData());

        public static EmptyTableData Instance => lazy.Value;

        private EmptyTableData()
        {
            Items = Array.Empty<T>();
            TotalItems = 0;
        }
    }
}
public class JingetTableDataBindModel
{
    public string SearchTerm { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string SortColumn { get; set; }
    public OrderByDirection SortDirection { get; set; } = OrderByDirection.Ascending;
    public JingetTableEventType EventType { get; set; }
}
public enum JingetTableEventType
{
    None, Search, Sort, Pagination
}

