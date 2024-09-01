using Jinget.Blazor.Models.JingetTable;

namespace Jinget.Blazor.Components.Table;

public abstract partial class JingetTableBase<T> : JingetBaseComponent
{
    /// <summary>
    /// Defines whether to select row on row click or not.
    /// </summary>
    [Parameter] public bool RowIsSelectable { get; set; }

    /// <summary>
    /// Defines whether to show selected row in badge bar or not. default is 'false'
    /// </summary>
    [Parameter] public bool ShowSelectedRowInBadgeBar { get; set; } = false;

    /// <summary>
    /// Selected row label text in badge bar. default is 'سطر انتخابی:'
    /// </summary>
    [Parameter] public string SelectedRowBadgeTextFormat { get; set; } = "سطر انتخابی: ";

    /// <summary>
    /// Css class used to style the selected row. default is 'table-info'
    /// </summary>
    [Parameter] public string SelectedRowCss { get; set; } = "table-info";

    /// <summary>
    /// Event which is fired whenever the user selects a row in table
    /// </summary>
    [Parameter] public EventCallback<T?> OnRowSelected { get; set; }

    protected T selectedRow;

    protected async Task SelectRowAsync(T? row)
    {
        if (!RowIsSelectable)
        {
            await Task.CompletedTask;
        }
        else
        {
            selectedRow = row;
            await OnRowSelected.InvokeAsync(row);
        }
    }

    protected bool HasSelectedRow() => ShowSelectedRowInBadgeBar && selectedRow != null;

    protected async Task ClearSelectedRowAsync() => await SelectRowAsync(default);

    protected JingetTableModel<T> GetSelectedRowModel() => new([selectedRow], 1);
}
