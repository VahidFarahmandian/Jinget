using Jinget.Blazor.Enums;
using Jinget.Core.Enumerations;
using Jinget.Core.ExtensionMethods;

namespace Jinget.Blazor.Components.Table;

public abstract partial class JingetTableBase<T> : JingetBaseComponent
{
    /// <summary>
    /// Defines whether columns in table are sortable or not. 
    /// If columns are sortable then each sortable column should have Sortable=true in its attribute.
    /// default is 'true'
    /// </summary>
    [Parameter] public bool Sortable { get; set; } = true;

    /// <summary>
    /// sort badge text format. default is: 'مرتب شده براساس: {sort_col} بصورت {sort_dir}'
    /// </summary>
    [Parameter] public string SortBadgeTextFormat { get; set; } = "مرتب شده براساس: {sort_col} بصورت {sort_dir}";

    /// <summary>
    /// ascending sort badge text. default is 'صعودی'
    /// </summary>
    [Parameter] public string AscendingSortText { get; set; } = "صعودی";

    /// <summary>
    /// descending sort badge text. default is 'صعودی'
    /// </summary>
    [Parameter] public string DescendingSortText { get; set; } = "نزولی";

    protected OrderByDirection? currentSortDirection = null;
    protected string? currentSortColumnName = null;

    OrderByDirection GetSortDirection() => currentSortDirection ?? OrderByDirection.Ascending;
    string GetSortColumn() => string.IsNullOrWhiteSpace(currentSortColumnName) ? "" : currentSortColumnName;

    protected bool HasSort() => !string.IsNullOrWhiteSpace(currentSortColumnName) && currentSortDirection != null;
    protected string GetDataRowCssClass(T row) => selectedRow != null && row != null && selectedRow.HasSameValuesAs(row) ? SelectedRowCss : "";
    protected string GetSortIcon(string columnName)
        => currentSortColumnName == columnName
        ? currentSortDirection switch
        {
            OrderByDirection.Ascending => "sort-desc",
            OrderByDirection.Descending => "sort-asc",
            _ => "sort"
        }
        : "sort";

    protected string GetSortBadgeText()
    {
        string? colDisplayText = _columns.FirstOrDefault(x => x.Name == currentSortColumnName)?.DisplayText;
        string sortDirectionText = GetSortDirection() == OrderByDirection.Ascending ? AscendingSortText : DescendingSortText;
        return SortBadgeTextFormat
            .Replace("{sort_col}", colDisplayText)
            .Replace("{sort_dir}", sortDirectionText);
    }

    protected async Task ClearSortAsync()
    {
        currentSortDirection = null;
        await SortAsync(true, "", true);
    }

    protected async Task SortAsync(bool sortable, string name, bool reset = false)
    {
        if (!sortable)
            return;
        currentSortColumnName = name;
        if (!reset)
        {
            if (currentSortDirection == null || currentSortDirection == OrderByDirection.Descending)
                currentSortDirection = OrderByDirection.Ascending;
            else
                currentSortDirection = OrderByDirection.Descending;
        }

        currentPageIndex = 1;
        await BindAsync(JingetTableEventType.Sort);
    }
}
